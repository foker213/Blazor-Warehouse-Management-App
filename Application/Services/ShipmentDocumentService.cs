using ErrorOr;
using Mapster;
using System.Data.Common;
using WarehouseManagement.Application.IRepositories;
using WarehouseManagement.Application.IServices;
using WarehouseManagement.Contracts;
using WarehouseManagement.Contracts.Shipment;
using WarehouseManagement.Domain.Enums;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Application.Services;

internal sealed class ShipmentDocumentService : IShipmentDocumentService
{
    private readonly IShipmentDocumentRepository _documentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBalanceRepository _balanceRepository;
    private readonly ISynchronizationService _syncService;

    public ShipmentDocumentService(IShipmentDocumentRepository documentRepository, IUnitOfWork unitOfWork, IBalanceRepository balanceRepository, ISynchronizationService syncService)
    {
        _documentRepository = documentRepository;
        _unitOfWork = unitOfWork;
        _balanceRepository = balanceRepository;
        _syncService = syncService;
    }

    public async Task<List<ShipmentDocumentDto>> GetAll(CancellationToken ct)
    {
        List<ShipmentDocument> items = await _documentRepository.GetAllAsync(ct);

        return items.Adapt<List<ShipmentDocumentDto>>();
    }

    public async Task<ErrorOr<ShipmentDocumentUpdateDto>> GetBy(int id, CancellationToken ct)
    {
        ShipmentDocument? shipmentDocument = await _documentRepository.GetByAsync(id, ct);

        if (shipmentDocument is null)
            return ErrorOr<ShipmentDocumentUpdateDto>.From(new List<Error> { Error.NotFound() });

        return shipmentDocument.Adapt<ShipmentDocumentUpdateDto>();
    }

    public async Task<ErrorOr<Created>> CreateAsync(ShipmentDocumentCreateDto shipment, CancellationToken ct)
    {
        ErrorOr<ShipmentDocument> documentResult = await CreateDocumentAsync(shipment, ct);

        if (documentResult.IsError)
            return documentResult.Errors.First();

        ShipmentDocument document = documentResult.Value;

        foreach(ShipmentResourceCreateDto shipmentResource in shipment.ShipmentResources)
        {
            ErrorOr<ShipmentResource> addResult = document.AddResource(shipmentResource.ResourceId, shipmentResource.UnitOfMeasureId, shipmentResource.Quantity);
            if (addResult.IsError)
                return addResult.Errors.First();
        }

        await _documentRepository.CreateAsync(document);
        return Result.Created;
    }

    public async Task<ErrorOr<Updated>> UpdateAsync(ShipmentDocumentUpdateDto shipment, CancellationToken ct)
    {
        ShipmentDocument? existingDocument = await _documentRepository.GetByAsync(shipment.Id, ct);
        if (existingDocument is null)
            return Error.NotFound("DocumentNotFound", "Документ не найден");

        existingDocument.Update(shipment.Number, DateOnly.FromDateTime(shipment.Date), shipment.ClientId);

        List<ShipmentResource> deletedShipmentResources;

        if (existingDocument.Status != Status.Signed)
        {
            deletedShipmentResources = existingDocument.ShipmentResources
                .Where(r => !shipment.ShipmentResources.Any(k =>
                    k.Id == r.Id && k.Id != 0 && k.UnitOfMeasureId == r.UnitOfMeasureId && k.ResourceId == r.ResourceId))
                .ToList();

            existingDocument.RemoveResources(deletedShipmentResources);

            foreach (ShipmentResourceUpdateDto shipmentResource in shipment.ShipmentResources)
            {
                ErrorOr<ShipmentResource> resource = existingDocument.AddResource(
                    shipmentResource.ResourceId,
                    shipmentResource.UnitOfMeasureId,
                    shipmentResource.Quantity,
                    shipmentResource.Id);

                if (resource.IsError)
                    return resource.Errors;
            }

            await _documentRepository.UpdateAsync(existingDocument);
            return Result.Updated;
        }

        Dictionary<(int ResourceId, int UnitId), int> balanceChanges = new();
        List<Balance> balancesToUpdate = new();

        deletedShipmentResources = existingDocument.ShipmentResources
            .Where(r => !shipment.ShipmentResources.Any(k =>
                k.Id == r.Id && k.Id != 0 && k.UnitOfMeasureId == r.UnitOfMeasureId && k.ResourceId == r.ResourceId))
            .ToList();

        existingDocument.RemoveResources(deletedShipmentResources);

        foreach (ShipmentResource deletedResource in deletedShipmentResources)
        {
            var key = (deletedResource.ResourceId, deletedResource.UnitOfMeasureId);
            balanceChanges[key] = balanceChanges.GetValueOrDefault(key) + deletedResource.Quantity;
        }

        foreach (ShipmentResourceUpdateDto shipmentResource in shipment.ShipmentResources)
        {
            ErrorOr<ShipmentResource> resource = existingDocument.AddResource(
                shipmentResource.ResourceId,
                shipmentResource.UnitOfMeasureId,
                shipmentResource.Quantity,
                shipmentResource.Id);

            if (resource.IsError)
                return resource.Errors;

            var key = (shipmentResource.ResourceId, shipmentResource.UnitOfMeasureId);
            balanceChanges[key] = balanceChanges.GetValueOrDefault(key) - resource.Value.RecalculateDifference();
        }

        foreach (var change in balanceChanges)
        {
            Balance? balance = await _balanceRepository.GetByIdsAsync(
                change.Key.ResourceId,
                change.Key.UnitId,
                ct);

            if (balance is null)
            {
                ErrorOr<Balance> createResult = Balance.Create(change.Key.ResourceId, change.Key.UnitId, 0);
                if (createResult.IsError)
                    return createResult.Errors;

                balance = createResult.Value;
            }

            int newQuantity = balance.Quantity + change.Value;

            if (newQuantity < 0)
            {
                return Error.Conflict("InsufficientBalance",
                    $"Недостаточно ресурсов (ID: {change.Key.ResourceId}) на складе. Требуется: {-change.Value}, доступно: {balance.Quantity}");
            }

            ErrorOr<Success> updateResult = balance.ChangeQueantity(newQuantity);
            if (updateResult.IsError)
                return updateResult.Errors;

            balancesToUpdate.Add(balance);
        }

        List<IDisposable> locks = await GetInLineAsync(existingDocument.ShipmentResources.ToList());

        try
        {
            await using DbTransaction? transaction = await _unitOfWork.BeginTransactionAsync(ct);

            _documentRepository.Update(existingDocument);

            foreach (Balance balance in balancesToUpdate)
            {
                if (balance.CheckExistInDb())
                    _balanceRepository.Update(balance);
                else
                    _balanceRepository.Add(balance);
            }

            Error? errorWhenSave = await _unitOfWork.CommitTransactionAsync(ct);
            return errorWhenSave is null
                ? Result.Updated
                : errorWhenSave.Value;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            return Error.Conflict("UnknownException", "Произошла непредвиденная ошибка");
        }
        finally
        {
            foreach (IDisposable lockObj in locks)
                lockObj.Dispose();
        }
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(int id, CancellationToken ct)
    {
        ShipmentDocument? existingDocument = await _documentRepository.GetByAsync(id, ct);

        if (existingDocument is null)
            return Error.NotFound("DocumentNotFound", "Документ не найден");


        if (existingDocument.Status == Status.NotSigned)
        {
            await _documentRepository.DeleteAsync(existingDocument, ct);
            return Result.Deleted;
        }

        List<Balance>? balances = new();

        foreach (ShipmentResource shipmentResource in existingDocument.ShipmentResources)
        {
            Balance? balance = await _balanceRepository.GetByIdsAsync(shipmentResource.ResourceId, shipmentResource.UnitOfMeasureId, ct);

            if (balance is not null)
            {
                int totalQuantity;
                ErrorOr<Success> result;

                Balance? addedBalance = balances.FirstOrDefault(x => x.ResourceId == shipmentResource.ResourceId &&
                                                       x.UnitOfMeasureId == shipmentResource.UnitOfMeasureId);

                if (addedBalance is not null)
                {
                    totalQuantity = addedBalance.Quantity + shipmentResource.Quantity;
                    result = addedBalance.ChangeQueantity(totalQuantity);

                    if (result.IsError)
                        return result.Errors.First();
                }
                else
                {
                    totalQuantity = balance.Quantity + shipmentResource.Quantity;
                    result = balance.ChangeQueantity(totalQuantity);

                    if (result.IsError)
                        return result.Errors.First();

                    balances.Add(balance);
                }
            }
        }

        List<IDisposable> locks = await GetInLineAsync(existingDocument.ShipmentResources.ToList());

        try
        {
            await using DbTransaction? transaction = await _unitOfWork.BeginTransactionAsync(ct);

            try
            {
                _documentRepository.Remove(existingDocument);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            foreach (Balance balance in balances)
            {
                if (balance.Id == 0)
                    _balanceRepository.Add(balance);
                else
                    _balanceRepository.Update(balance);
            }

            Error? errorWhenSave = await _unitOfWork.CommitTransactionAsync(ct);
            return errorWhenSave is null
                ? Result.Deleted
                : errorWhenSave.Value;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            return Error.Conflict("UnknownException", "Произошла непредвиденная ошибка");
        }
        finally
        {
            foreach (var lockObj in locks)
                lockObj.Dispose();
        }
    }

    public async Task<ErrorOr<Updated>> ChangeStatusAsync(ShipmentDocumentUpdateDto shipment, CancellationToken ct)
    {
        ShipmentDocument? existingDocument = await _documentRepository.GetByAsync(shipment.Id, ct);
        if (existingDocument is null)
            return Error.NotFound("DocumentNotFound", "Документ не найден");

        List<ShipmentResource> currentResources = existingDocument.ShipmentResources.ToList();

        existingDocument.Update(shipment.Number, DateOnly.FromDateTime(shipment.Date), shipment.ClientId, true);

        Dictionary<(int ResourceId, int UnitId), int> balanceChanges = new();
        List<Balance> balancesToUpdate = new();

        if (existingDocument.Status == Status.Signed)
        {
            foreach (var resourceDto in shipment.ShipmentResources)
            {
                var key = (resourceDto.ResourceId, resourceDto.UnitOfMeasureId);
                balanceChanges[key] = balanceChanges.GetValueOrDefault(key) - resourceDto.Quantity;
            }
        }
        else
        {
            foreach (var resource in currentResources)
            {
                var key = (resource.ResourceId, resource.UnitOfMeasureId);
                balanceChanges[key] = balanceChanges.GetValueOrDefault(key) + resource.Quantity;
            }
        }

        foreach (var change in balanceChanges)
        {
            Balance? balance = await _balanceRepository.GetByIdsAsync(
                change.Key.ResourceId,
                change.Key.UnitId,
                ct);

            if (existingDocument.Status == Status.Signed)
            {
                if (balance is null)
                    return Error.Conflict("BalanceNotFound",
                        $"Ресурс (ID: {change.Key.ResourceId}) не найден на складе");

                int newQuantity = balance.Quantity + change.Value;
                if (newQuantity < 0)
                    return Error.Conflict("InsufficientBalance",
                        $"Недостаточно ресурсов (ID: {change.Key.ResourceId}) на складе. Требуется: {-change.Value}, доступно: {balance.Quantity}");
            }
            else
            {
                if (balance is null)
                {
                    ErrorOr<Balance> createResult = Balance.Create(change.Key.ResourceId, change.Key.UnitId, 0);
                    if (createResult.IsError)
                        return createResult.Errors;

                    balance = createResult.Value;
                }
            }

            ErrorOr<Success> updateResult = balance.ChangeQueantity(balance.Quantity + change.Value);
            if (updateResult.IsError)
                return updateResult.Errors;

            balancesToUpdate.Add(balance);
        }

        List<ShipmentResource> deletedShipmentResources = currentResources
            .Where(r => !shipment.ShipmentResources.Any(k =>
                k.Id == r.Id && k.Id != 0 && k.UnitOfMeasureId == r.UnitOfMeasureId && k.ResourceId == r.ResourceId))
            .ToList();

        existingDocument.RemoveResources(deletedShipmentResources);

        foreach (ShipmentResourceUpdateDto shipmentResource in shipment.ShipmentResources)
        {
            ErrorOr<ShipmentResource> resourceResult = existingDocument.AddResource(
                shipmentResource.ResourceId,
                shipmentResource.UnitOfMeasureId,
                shipmentResource.Quantity,
                shipmentResource.Id);

            if (resourceResult.IsError)
                return resourceResult.Errors;
        }

        List<IDisposable> locks = await GetInLineAsync(existingDocument.ShipmentResources.ToList());

        try
        {
            await using DbTransaction? transaction = await _unitOfWork.BeginTransactionAsync(ct);

            _documentRepository.Update(existingDocument);

            foreach (Balance balance in balancesToUpdate)
            {
                if (balance.CheckExistInDb())
                    _balanceRepository.Update(balance);
                else
                    _balanceRepository.Add(balance);
            }

            Error? errorWhenSave = await _unitOfWork.CommitTransactionAsync(ct);
            return errorWhenSave is null
                ? Result.Updated
                : errorWhenSave.Value;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            return Error.Conflict("UnknownException", "Произошла непредвиденная ошибка");
        }
        finally
        {
            foreach (var lockObj in locks)
                lockObj.Dispose();
        }
    }

    public async Task<List<ShipmentDocumentDto>> FilterAsync(FilterDto filter, CancellationToken ct)
    {
        List<ShipmentDocument> items = await _documentRepository.FilterAsync(filter, ct);

        return items.Adapt<List<ShipmentDocumentDto>>();
    }

    private async Task<List<IDisposable>> GetInLineAsync(List<ShipmentResource> shipmentResources)
    {
        shipmentResources = shipmentResources ?? new();

        var resourceLocks = shipmentResources
            .Where(x => x.Quantity >= 0)
            .Select(x => (x.ResourceId, x.UnitOfMeasureId))
            .Distinct()
            .ToList();

        resourceLocks.Sort();

        List<IDisposable> locks = new();

        foreach (var (resourceId, unitId) in resourceLocks)
        {
            IDisposable? lockObj = await _syncService.AcquireLockAsync(resourceId, unitId);
            locks.Add(lockObj);
        }

        return locks;
    }

    private async Task<ErrorOr<ShipmentDocument>> CreateDocumentAsync(
    ShipmentDocumentCreateDto shipment,
    CancellationToken ct)
    {
        if (shipment.ShipmentResources is null || shipment.ShipmentResources.Count == 0)
            return Error.Validation("ResourcesRequired", "Документ не содержит в себе ресурсы");

        ShipmentDocument? existingDocument = await _documentRepository.GetByNumberAsync(shipment.Number, ct);

        return ShipmentDocument.Create(
            shipment.Number,
            DateOnly.FromDateTime(shipment.Date),
            shipment.ClientId,
            existingDocument);
    }
}