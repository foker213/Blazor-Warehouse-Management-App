using ErrorOr;
using Mapster;
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

    public async Task<ErrorOr<ShipmentDocumentDto>> GetBy(int id, CancellationToken ct)
    {
        ShipmentDocument? shipmentDocument = await _documentRepository.GetByAsync(id, ct);

        if (shipmentDocument is null)
            return ErrorOr<ShipmentDocumentDto>.From(new List<Error> { Error.NotFound() });

        return shipmentDocument.Adapt<ShipmentDocumentDto>();
    }

    public async Task<ErrorOr<Created>> CreateAsync(ShipmentDocumentCreateDto shipment, CancellationToken ct)
    {
        ErrorOr<ShipmentDocument> documentResult = await CreateDocumentAsync(shipment, ct);

        if (documentResult.IsError)
            return documentResult.Errors.First();

        ShipmentDocument document = documentResult.Value;

        foreach(ShipmentResourceCreateDto shipmentResource in shipment.ShipmentResources)
        {
            var addResult = document.AddResource(shipmentResource.ResourceId, shipmentResource.UnitOfMeasureId, shipmentResource.Quantity);
            if (addResult.IsError)
                return addResult.Errors.First();
        }

        await _documentRepository.CreateAsync(document);
        return Result.Created;
    }

    public async Task<ErrorOr<Updated>> UpdateAsync(ShipmentDocumentDto shipment, CancellationToken ct)
    {
        ShipmentDocument? existingDocument = await _documentRepository.GetByAsync(shipment.Id, ct);
        if (existingDocument is null)
            return Error.NotFound("DocumentNotFound", "Документ не найден");

        List<ShipmentResource> deletedShipmentResources = new();
        List<Balance> balances = new();

        existingDocument.Update(shipment.Number, DateOnly.FromDateTime(shipment.Date), shipment.Client.Id);

        deletedShipmentResources = existingDocument.ShipmentResources.Where(r => !shipment.ShipmentResources.Any(k =>
                    k.Id == r.Id && k.Id != 0))
                    .ToList();

        existingDocument.RemoveResources(deletedShipmentResources);

        foreach (ShipmentResourceDto shipmentResource in shipment.ShipmentResources)
        {
            existingDocument.AddResource(shipmentResource.Resource.Id, shipmentResource.UnitOfMeasure.Id, shipmentResource.Quantity, shipmentResource.Id);

            if (existingDocument.Status == Status.Signed)
            {
                Balance? balance = await _balanceRepository.GetByIdsAsync(shipmentResource.Resource.Id, shipmentResource.UnitOfMeasure.Id, ct);

                if (balance is not null)
                {
                    int totalQuantity = balance.Quantity - existingDocument.ShipmentResources.Last().RecalculateDifference();

                    ErrorOr<Success> result = balance.ChangeQueantity(totalQuantity);

                    if (result.IsError)
                        return result.Errors.First();

                    balances.Add(balance);
                }
                else
                {
                    return Error.Conflict("BalanceNotFound", 
                        $"На складе станет недостаточно выбранных ресурсов: {shipmentResource.Resource.Name} в {shipmentResource.UnitOfMeasure.Name}");
                }
            }
        }

        if (existingDocument.Status == Status.NotSigned)
        {
            await _documentRepository.UpdateAsync(existingDocument);
            return Result.Updated;
        }
        else
        {
            foreach (ShipmentResource shipmentResource in deletedShipmentResources)
            {
                Balance? balance = await _balanceRepository.GetByIdsAsync(shipmentResource.ResourceId, shipmentResource.UnitOfMeasureId, ct);

                if (balance is not null)
                {
                    ErrorOr<Success> result = balance.Increase(shipmentResource.Quantity);

                    if (result.IsError)
                        return result.Errors.First();

                    balances.Add(balance);
                }
            }

            List<IDisposable> locks = await GetInLineAsync(existingDocument.ShipmentResources.ToList());

            try
            {
                await using var transaction = await _unitOfWork.BeginTransactionAsync(ct);

                _documentRepository.Update(existingDocument);

                foreach (Balance balance in balances)
                {
                    _balanceRepository.Update(balance);
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
                int totalQuantity = balance.Quantity + shipmentResource.Quantity;

                ErrorOr<Success> result = balance.ChangeQueantity(totalQuantity);

                if (result.IsError)
                    return result.Errors.First();

                balances.Add(balance);
            }
        }

        List<IDisposable> locks = await GetInLineAsync(existingDocument.ShipmentResources.ToList());

        try
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync(ct);

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

    public async Task<ErrorOr<Updated>> ChangeStatusAsync(ShipmentDocumentDto shipment, CancellationToken ct)
    {
        ShipmentDocument? existingDocument = await _documentRepository.GetByAsync(shipment.Id, ct);
        if (existingDocument is null)
            return Error.NotFound("DocumentNotFound", "Документ не найден");

        List<ShipmentResource> deletedShipmentResources = new();
        List<Balance> balances = new();

        existingDocument.Update(shipment.Number, DateOnly.FromDateTime(shipment.Date), shipment.Client.Id, true);

        deletedShipmentResources = existingDocument.ShipmentResources.Where(r => !shipment.ShipmentResources.Any(k =>
                    k.Id == r.Id && k.Id != 0))
                    .ToList();

        existingDocument.RemoveResources(deletedShipmentResources);

        foreach (ShipmentResourceDto shipmentResource in shipment.ShipmentResources)
        {
            existingDocument.AddResource(shipmentResource.Resource.Id, shipmentResource.UnitOfMeasure.Id, shipmentResource.Quantity, shipmentResource.Id);

            Balance? balance = await _balanceRepository.GetByIdsAsync(shipmentResource.Resource.Id, shipmentResource.UnitOfMeasure.Id, ct);

            if (existingDocument.Status == Status.Signed)
            {
                if (balance is not null && balance.Quantity != 0)
                {
                    int totalQuantity = balance.Quantity - existingDocument.ShipmentResources.Last().Quantity;

                    ErrorOr<Success> result = balance.ChangeQueantity(totalQuantity);

                    if (result.IsError)
                        return result.Errors.First();

                    balances.Add(balance);
                }
                else
                {
                    return Error.Conflict("BalanceNotFound",
                        $"На складе станет недостаточно выбранных ресурсов: {shipmentResource.Resource.Name} в {shipmentResource.UnitOfMeasure.Name}");
                }
            }
            else
            {
                if (balance is not null && balance.Quantity != 0)
                {
                    int totalQuantity = balance.Quantity + existingDocument.ShipmentResources.Last().Quantity;

                    ErrorOr<Success> result = balance.ChangeQueantity(totalQuantity);

                    if (result.IsError)
                        return result.Errors.First();

                    balances.Add(balance);
                }
            }
        }

        foreach (ShipmentResource shipmentResource in deletedShipmentResources)
        {
            Balance? balance = await _balanceRepository.GetByIdsAsync(shipmentResource.ResourceId, shipmentResource.UnitOfMeasureId, ct);

            if (existingDocument.Status == Status.NotSigned)
            {
                if (balance is not null)
                {
                    ErrorOr<Success> result = balance.Increase(shipmentResource.Quantity);

                    if (result.IsError)
                        return result.Errors.First();

                    balances.Add(balance);
                }
            }
        }

        List<IDisposable> locks = await GetInLineAsync(existingDocument.ShipmentResources.ToList());

        try
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync(ct);

            _documentRepository.Update(existingDocument);

            foreach (Balance balance in balances)
            {
                _balanceRepository.Update(balance);
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
            .Where(x => x.Quantity > 0)
            .Select(x => (x.ResourceId, x.UnitOfMeasureId))
            .Distinct()
            .ToList();

        resourceLocks.Sort();

        List<IDisposable> locks = new List<IDisposable>();

        foreach (var (resourceId, unitId) in resourceLocks)
        {
            var lockObj = await _syncService.AcquireLockAsync(resourceId, unitId);
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