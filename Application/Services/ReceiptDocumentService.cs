using ErrorOr;
using Mapster;
using System.Data.Common;
using WarehouseManagement.Application;
using WarehouseManagement.Application.IRepositories;
using WarehouseManagement.Application.IServices;
using WarehouseManagement.Contracts;
using WarehouseManagement.Contracts.Receipt;
using WarehouseManagement.Domain.Models;

internal sealed class ReceiptDocumentService : IReceiptDocumentService
{
    private readonly IReceiptDocumentRepository _documentRepository;
    private readonly IReceiptResourceService _resourceService;
    private readonly IBalanceService _balanceService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISynchronizationService _syncService;


    public ReceiptDocumentService(
        IReceiptDocumentRepository documentRepository,
        IReceiptResourceService resourceService,
        IBalanceService balanceService,
        IUnitOfWork unitOfWork,
        ISynchronizationService syncService)
    {
        _documentRepository = documentRepository;
        _resourceService = resourceService;
        _balanceService = balanceService;
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
        return client is null
            ? Error.NotFound()
            : client.Adapt<ReceiptDocumentUpdateDto>();
    }

    public async Task<ErrorOr<Created>> CreateAsync(ReceiptDocumentCreateDto receipt, CancellationToken ct)
    {
        ErrorOr<ReceiptDocument> documentResult = await CreateDocumentAsync(receipt, ct);
        if (documentResult.IsError) return documentResult.Errors.First();

        ReceiptDocument document = documentResult.Value;

        if (receipt.ReceiptResources is null || receipt.ReceiptResources.Count == 0)
        {
            await _documentRepository.CreateAsync(document);
            return Result.Created;
        }

        List<IDisposable> locks = new();

        try
        {
            var futureChanges = receipt.ReceiptResources
                .Select(x => (x.ResourceId, x.UnitOfMeasureId))
                .ToList();

            locks = await AcquireLocksForBalanceChanges(futureChanges);

            ErrorOr<Success> result = _resourceService.ProcessResources(document, receipt.ReceiptResources);

            if (result.IsError)
                return result.Errors.First();

            ErrorOr<List<Balance>> balancesResult = await _balanceService.ProcessBalancesAsync(
                document,
                receipt.ReceiptResources,
                ct);

            if (balancesResult.IsError)
                return balancesResult.Errors.First();

            return await SaveDocumentWithBalances(document, balancesResult.Value, ct);
        }
        catch
        {
            return Error.Conflict("TransactionFailed", $"Ошибка при изменении балансов");
        }
        finally
        {
            foreach (IDisposable lockObj in locks)
                lockObj.Dispose();
        }
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

        List<IDisposable> locks = new();

        try
        {
            var futureChanges = receipt.ReceiptResources
                .Select(x => (x.ResourceId, x.UnitOfMeasureId))
                .ToList();

            locks = await AcquireLocksForBalanceChanges(futureChanges);

            ErrorOr<List<ReceiptResource>> result = _resourceService.ProcessResourceUpdates(existingDocument, receipt.ReceiptResources);

            if (result.IsError)
                return result.Errors.First();

            List<ReceiptResource> deletedReceiptsResource = result.Value;

            ErrorOr<List<Balance>> balancesResult = await _balanceService.ProcessBalanceUpdatesAsync(
                existingDocument,
                existingDocument.ReceiptResources.ToList(),
                deletedReceiptsResource,
                ct);

            if (balancesResult.IsError) 
                return balancesResult.Errors.First();

            return await UpdateDocumentWithBalances(existingDocument, balancesResult.Value, ct);
        }
        catch
        {
            return Error.Conflict("TransactionFailed", $"Ошибка при изменении балансов");
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

        if (existingDocument.ReceiptResources.Count == 0)
        {
            await _documentRepository.DeleteAsync(existingDocument, ct);
            return Result.Deleted;
        }

        List<IDisposable> locks = new();

        try
        {
            var futureChanges = existingDocument.ReceiptResources
                .Select(x => (x.ResourceId, x.UnitOfMeasureId))
                .ToList();

            locks = await AcquireLocksForBalanceChanges(futureChanges);

            ErrorOr<List<Balance>> balancesResult = await _balanceService.ProcessBalanceUpdatesAsync(existingDocument,
                                                                                                     new List<ReceiptResource>(),
                                                                                                     existingDocument.ReceiptResources.ToList(),
                                                                                                     ct);

            if (balancesResult.IsError) return balancesResult.Errors.First();

            return await DeleteDocumentWithBalances(existingDocument, balancesResult.Value, ct);
        }
        catch
        {
            return Error.Conflict("TransactionFailed", $"Ошибка при изменении балансов");
        }
        finally
        {
            foreach (IDisposable lockObj in locks)
                lockObj.Dispose();
        }
    }

    public async Task<List<ReceiptDocumentDto>> FilterAsync(FilterDto filter, CancellationToken ct)
    {
        List<ReceiptDocument> items = await _documentRepository.FilterAsync(filter, ct);
        return items.Adapt<List<ReceiptDocumentDto>>();
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

    private async Task<ErrorOr<Created>> SaveDocumentWithBalances(
        ReceiptDocument document,
        List<Balance> balances,
        CancellationToken ct)
    {
        await using DbTransaction? transaction = await _unitOfWork.BeginTransactionAsync(ct);

        _documentRepository.Add(document);

        _balanceService.Add(balances);

        Error? errorWhenSave = await _unitOfWork.CommitTransactionAsync(ct);
        return errorWhenSave is null ? Result.Created : errorWhenSave.Value;
    }

    private async Task<ErrorOr<Updated>> UpdateDocumentWithBalances(
        ReceiptDocument document,
        List<Balance> balances,
        CancellationToken ct)
    {
        await using DbTransaction? transaction = await _unitOfWork.BeginTransactionAsync(ct);

        _documentRepository.Update(document);

        _balanceService.Update(balances);

        Error? errorWhenSave = await _unitOfWork.CommitTransactionAsync(ct);
        return errorWhenSave is null ? Result.Updated : errorWhenSave.Value;
    }

    private async Task<ErrorOr<Deleted>> DeleteDocumentWithBalances(
        ReceiptDocument document,
        List<Balance> balances,
        CancellationToken ct)
    {
        await using DbTransaction? transaction = await _unitOfWork.BeginTransactionAsync(ct);

        _documentRepository.Remove(document);

        _balanceService.Remove(balances);

        Error? errorWhenSave = await _unitOfWork.CommitTransactionAsync(ct);
        return errorWhenSave is null ? Result.Deleted : errorWhenSave.Value;
    }

    private async Task<List<IDisposable>> AcquireLocksForBalanceChanges(
        List<(int ResourceId, int UnitId)> changes)
    {
        var distinctChanges = changes.Distinct().ToList();
        distinctChanges.Sort();

        List<IDisposable> locks = new();
        foreach (var (resourceId, unitId) in distinctChanges)
        {
            var lockObj = await _syncService.AcquireLockAsync(resourceId, unitId);
            locks.Add(lockObj);
        }

        return locks;
    }
}