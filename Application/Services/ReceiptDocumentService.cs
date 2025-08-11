using ErrorOr;
using Mapster;
using WarehouseManagement.Application.IRepositories;
using WarehouseManagement.Application.IServices;
using WarehouseManagement.Contracts;
using WarehouseManagement.Contracts.Receipt;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Application.Services;

internal sealed class ReceiptDocumentService : IReceiptDocumentService
{
    private readonly IReceiptDocumentRepository _documentRepository;
    private readonly IBalanceRepository _balanceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISynchronizationService _syncService;

    public ReceiptDocumentService(
        IReceiptDocumentRepository documentRepository,
        IBalanceRepository balanceRepository,
        IUnitOfWork unitOfWork,
        ISynchronizationService syncService)
    {
        _documentRepository = documentRepository;
        _balanceRepository = balanceRepository;
        _unitOfWork = unitOfWork;
        _syncService = syncService;
    }

    public async Task<List<ReceiptDocumentDto>> GetAll(CancellationToken ct)
    {
        List<ReceiptDocument> items = await _documentRepository.GetAllAsync(ct);

        return items.Adapt<List<ReceiptDocumentDto>>();
    }

    public async Task<ErrorOr<ReceiptDocumentUpdateDto>> GetBy(int id, CancellationToken ct)
    {
        ReceiptDocument? client = await _documentRepository.GetByAsync(id, ct);

        if (client is null)
            return ErrorOr<ReceiptDocumentUpdateDto>.From(new List<Error> { Error.NotFound() });

        return client.Adapt<ReceiptDocumentUpdateDto>();
    }

    public async Task<ErrorOr<Created>> CreateAsync(ReceiptDocumentCreateDto receipt, CancellationToken ct)
    {
        ErrorOr<ReceiptDocument> documentResult = await CreateDocumentAsync(receipt, ct);

        if (documentResult.IsError)
            return documentResult.Errors.First();

        ReceiptDocument document = documentResult.Value;

        if (receipt.ReceiptResources is null || receipt.ReceiptResources.Count == 0)
        {
            await _documentRepository.CreateAsync(document);
            return Result.Created;
        }

        List<Balance> balances = new();

        ErrorOr<List<Balance>> balancesResult = await ProcessResourcesAsync(document, receipt.ReceiptResources, ct);
        if (balancesResult.IsError)
            return balancesResult.Errors.First();

        balances = balancesResult.Value;

        ErrorOr<Success> saveResult = await SaveDocumentAsync(document, balances, receipt.ReceiptResources, ct);
        if (saveResult.IsError)
            return saveResult.Errors.First();

        return Result.Created;
    }

    public async Task<ErrorOr<Updated>> UpdateAsync(ReceiptDocumentUpdateDto receipt, CancellationToken ct)
    {
        ReceiptDocument? existingDocument = await _documentRepository.GetByAsync(receipt.Id, ct);
        if (existingDocument is null)
            return Error.NotFound("DocumentNotFound", "Документ не найден");

        if (existingDocument.Number != receipt.Number)
        {
            ReceiptDocument? documentWithSameNumber = await _documentRepository.GetByNumberAsync(receipt.Number, ct);
            if (documentWithSameNumber is not null)
                return Error.Validation("NumberDuplicate", "Документ с таким номером уже существует");

            existingDocument.Update(receipt.Number, DateOnly.FromDateTime(receipt.Date));
        }

        List<ReceiptResource> deletedReceiptsResource = new();
        List<Balance> balances = new();

        if (receipt.ReceiptResources is null || receipt.ReceiptResources.Count == 0)
        {
            deletedReceiptsResource.AddRange(existingDocument.ReceiptResources);
            existingDocument.RemoveResources(deletedReceiptsResource);
        }
        else
        {
            deletedReceiptsResource = existingDocument.ReceiptResources.Where(r => !receipt.ReceiptResources.Any(k =>
                k.Id == r.Id && k.Id != 0))
                .ToList();

            existingDocument.RemoveResources(deletedReceiptsResource);

            foreach (ReceiptResourceUpdateDto receiptResource in receipt.ReceiptResources)
            {
                existingDocument.AddResource(receiptResource.ResourceId, receiptResource.UnitOfMeasureId, receiptResource.Quantity, receiptResource.Id);
            }
        }

        foreach (ReceiptResource receiptResource in existingDocument.ReceiptResources)
        {
            Balance? balance = await _balanceRepository.GetByIdsAsync(receiptResource.ResourceId, receiptResource.UnitOfMeasureId, ct);

            if (balance is not null)
            {
                int totalQuantity = balance.Quantity - receiptResource.RecalculateDifference();

                ErrorOr<Success> result = balance.ChangeQueantity(totalQuantity);

                if (result.IsError)
                    return result.Errors.First();

                balances.Add(balance);
            }
            else
            {
                if(balances.Any(x => x.UnitOfMeasureId == receiptResource.UnitOfMeasureId && x.ResourceId == receiptResource.ResourceId))
                {
                    ErrorOr<Success> result = balances.First(x => x.UnitOfMeasureId == receiptResource.UnitOfMeasureId && x.ResourceId == receiptResource.ResourceId)
                        .Increase(receiptResource.Quantity);

                    if(result.IsError)
                        return result.Errors.First();
                }
                else
                {
                    ErrorOr<Balance> createdBalance = Balance.Create(receiptResource.ResourceId, receiptResource.UnitOfMeasureId, receiptResource.Quantity);

                    if (createdBalance.IsError)
                        return createdBalance.Errors.First();

                    balance = createdBalance.Value;

                    balances.Add(balance);
                }
            }
        }

        foreach (ReceiptResource receiptResource in deletedReceiptsResource)
        {
            Balance? balance = await _balanceRepository.GetByIdsAsync(receiptResource.ResourceId, receiptResource.UnitOfMeasureId, ct);

            if (balance is not null)
            {
                int totalQuantity = balance.Quantity - receiptResource.Quantity;

                ErrorOr<Success> result = balance.ChangeQueantity(totalQuantity);

                if (result.IsError)
                    return result.Errors.First();

                balances.Add(balance);
            }
        }

        List<IDisposable> locks = await GetInLineAsync(existingDocument.ReceiptResources.ToList());

        try
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync(ct);

            _documentRepository.Update(existingDocument);

            foreach (Balance balance in balances)
            {
                if (balance.Quantity == 0)
                {
                    // получаем из репозитория ресурсов поставок по id ресурса и единицы элемент, если null, то удалить, если существует, то update с кол-вом 0.
                    _balanceRepository.Remove(balance);
                }
                else if (balance.Id == 0)
                    _balanceRepository.Add(balance);
                else
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

    public async Task<ErrorOr<Deleted>> DeleteAsync(int id, CancellationToken ct)
    {
        ReceiptDocument? existingDocument = await _documentRepository.GetByAsync(id, ct);
        if (existingDocument is null)
            return Error.NotFound("DocumentNotFound", "Документ не найден");


        if(existingDocument.ReceiptResources is null || existingDocument.ReceiptResources.Count == 0)
        {
            await _documentRepository.DeleteAsync(existingDocument, ct);
            return Result.Deleted;
        }

        List<Balance>? balances = new();

        foreach (ReceiptResource receiptResource in existingDocument.ReceiptResources)
        {
            Balance? balance = await _balanceRepository.GetByIdsAsync(receiptResource.ResourceId, receiptResource.UnitOfMeasureId, ct);

            if (balance is not null)
            {
                int totalQuantity = balance.Quantity - receiptResource.Quantity;

                ErrorOr<Success> result = balance.ChangeQueantity(totalQuantity);

                if (result.IsError)
                    return result.Errors.First();

                balances.Add(balance);
            }
        }

        List<IDisposable> locks = await GetInLineAsync(existingDocument.ReceiptResources.ToList());

        try
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync(ct);

            _documentRepository.Remove(existingDocument);

            foreach (Balance balance in balances)
            {
                if (balance.Quantity == 0)
                    // получаем из репозитория ресурсов поставок по id ресурса и единицы элемент, если null, то удалить, если существует, то update с кол-вом 0.
                    _balanceRepository.Remove(balance);
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

    public async Task<List<ReceiptDocumentDto>> FilterAsync(FilterDto filter, CancellationToken ct)
    {
        List<ReceiptDocument> items = await _documentRepository.FilterAsync(filter, ct);

        return items.Adapt<List<ReceiptDocumentDto>>();
    }

    private async Task<List<IDisposable>> GetInLineAsync(List<ReceiptResource>? receiptResources)
    {
        receiptResources = receiptResources ?? new();

        var resourceLocks = receiptResources
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

    private async Task<ErrorOr<ReceiptDocument>> CreateDocumentAsync(
    ReceiptDocumentCreateDto receipt,
    CancellationToken ct)
    {
        ReceiptDocument? existingDocument = await _documentRepository
            .GetByNumberAsync(receipt.Number, ct);

        return ReceiptDocument.Create(
            receipt.Number,
            DateOnly.FromDateTime(receipt.Date),
            existingDocument);
    }

    private async Task<ErrorOr<List<Balance>>> ProcessResourcesAsync(
        ReceiptDocument document,
        List<ReceiptResourceCreateDto> resources,
        CancellationToken ct)
    {
        List<Balance> balances = new();
        List<IDisposable> locks = new();

        try
        {
            locks = await GetInLineAsync(document.ReceiptResources.ToList());

            foreach (var resourceDto in resources)
            {
                var balanceResult = await ProcessSingleResource(document, resourceDto, ct);
                if (balanceResult.IsError) return balanceResult.Errors;

                balances.Add(balanceResult.Value);
            }

            return balances;
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

    private async Task<ErrorOr<Balance>> ProcessSingleResource(
        ReceiptDocument document,
        ReceiptResourceCreateDto resourceDto,
        CancellationToken ct)
    {
        var addResult = document.AddResource(resourceDto.ResourceId, resourceDto.UnitOfMeasureId, resourceDto.Quantity);
        if (addResult.IsError) return addResult.Errors;

        Balance? balance = await _balanceRepository
            .GetByIdsAsync(resourceDto.ResourceId, resourceDto.UnitOfMeasureId, ct);

        if (balance is not null)
        {
            balance.Increase(resourceDto.Quantity);
            return balance;
        }

        return Balance.Create(resourceDto.ResourceId, resourceDto.UnitOfMeasureId, resourceDto.Quantity);
    }

    private async Task<ErrorOr<Success>> SaveDocumentAsync(
        ReceiptDocument document,
        List<Balance> balances,
        List<ReceiptResourceCreateDto>? resources,
        CancellationToken ct)
    {
        await using var transaction = await _unitOfWork.BeginTransactionAsync(ct);

        _documentRepository.Add(document);

        foreach (var balance in balances)
        {
            bool existInDb = balance.CheckExistInDb();

            if (existInDb)
                _balanceRepository.Update(balance);
            else
                _balanceRepository.Add(balance);
        }

        Error? errorWhenSave = await _unitOfWork.CommitTransactionAsync(ct);
        return errorWhenSave is null
            ? Result.Success
            : errorWhenSave.Value;
    }
}