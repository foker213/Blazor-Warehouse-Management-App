using ErrorOr;
using Mapster;
using WarehouseManagement.Application;
using WarehouseManagement.Application.IRepositories;
using WarehouseManagement.Application.IServices;
using WarehouseManagement.Contracts;
using WarehouseManagement.Contracts.Receipt;
using WarehouseManagement.Domain.Models;

internal sealed class ReceiptDocumentService : IReceiptDocumentService
{
    private readonly IReceiptDocumentRepository _documentRepository;
    private readonly IBalanceRepository _balanceRepository;
    private readonly IReceiptResourceService _resourceService;
    private readonly IUnitOfWork _unitOfWork;

    public ReceiptDocumentService(
        IReceiptDocumentRepository documentRepository,
        IBalanceRepository balanceRepository,
        IReceiptResourceService resourceService,
        IUnitOfWork unitOfWork)
    {
        _documentRepository = documentRepository;
        _balanceRepository = balanceRepository;
        _resourceService = resourceService;
        _unitOfWork = unitOfWork;
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
        var documentResult = await CreateDocumentAsync(receipt, ct);
        if (documentResult.IsError) return documentResult.Errors.First();

        var document = documentResult.Value;

        if (receipt.ReceiptResources is null || receipt.ReceiptResources.Count == 0)
        {
            await _documentRepository.CreateAsync(document);
            return Result.Created;
        }

        var balancesResult = await _resourceService.ProcessResourcesAsync(document, receipt.ReceiptResources, ct);
        if (balancesResult.IsError) return balancesResult.Errors.First();

        return await SaveDocumentWithBalances(document, balancesResult.Value, ct);
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

        if (receipt.ReceiptResources is null || receipt.ReceiptResources.Count == 0)
        {
            deletedReceiptsResource.AddRange(existingDocument.ReceiptResources);
            existingDocument.RemoveResources(deletedReceiptsResource);
        }
        else
        {
            deletedReceiptsResource = existingDocument.ReceiptResources
                .Where(r => !receipt.ReceiptResources.Any(k =>
                    k.Id == r.Id && k.Id != 0 &&
                    k.UnitOfMeasureId == r.UnitOfMeasureId &&
                    k.ResourceId == r.UnitOfMeasureId))
                .ToList();

            existingDocument.RemoveResources(deletedReceiptsResource);

            foreach (var receiptResource in receipt.ReceiptResources)
            {
                existingDocument.AddResource(
                    receiptResource.ResourceId,
                    receiptResource.UnitOfMeasureId,
                    receiptResource.Quantity,
                    receiptResource.Id);
            }
        }

        var balancesResult = await _resourceService.ProcessResourceUpdatesAsync(
            existingDocument,
            existingDocument.ReceiptResources.ToList(),
            deletedReceiptsResource,
            ct);

        if (balancesResult.IsError) return balancesResult.Errors.First();

        return await UpdateDocumentWithBalances(existingDocument, balancesResult.Value, ct);
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

        var balancesResult = await _resourceService.ProcessResourceUpdatesAsync(
            existingDocument,
            new List<ReceiptResource>(),
            existingDocument.ReceiptResources.ToList(),
            ct);

        if (balancesResult.IsError) return balancesResult.Errors.First();

        return await DeleteDocumentWithBalances(existingDocument, balancesResult.Value, ct);
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
        await using var transaction = await _unitOfWork.BeginTransactionAsync(ct);

        _documentRepository.Add(document);

        foreach (var balance in balances)
        {
            if (balance.CheckExistInDb())
                _balanceRepository.Update(balance);
            else
                _balanceRepository.Add(balance);
        }

        Error? errorWhenSave = await _unitOfWork.CommitTransactionAsync(ct);
        return errorWhenSave is null ? Result.Created : errorWhenSave.Value;
    }

    private async Task<ErrorOr<Updated>> UpdateDocumentWithBalances(
        ReceiptDocument document,
        List<Balance> balances,
        CancellationToken ct)
    {
        await using var transaction = await _unitOfWork.BeginTransactionAsync(ct);

        _documentRepository.Update(document);

        foreach (var balance in balances)
        {
            if (balance.Quantity == 0)
                _balanceRepository.Remove(balance);
            else if (balance.Id == 0)
                _balanceRepository.Add(balance);
            else
                _balanceRepository.Update(balance);
        }

        Error? errorWhenSave = await _unitOfWork.CommitTransactionAsync(ct);
        return errorWhenSave is null ? Result.Updated : errorWhenSave.Value;
    }

    private async Task<ErrorOr<Deleted>> DeleteDocumentWithBalances(
        ReceiptDocument document,
        List<Balance> balances,
        CancellationToken ct)
    {
        await using var transaction = await _unitOfWork.BeginTransactionAsync(ct);

        _documentRepository.Remove(document);

        foreach (var balance in balances)
        {
            if (balance.Quantity == 0)
                _balanceRepository.Remove(balance);
            else
                _balanceRepository.Update(balance);
        }

        Error? errorWhenSave = await _unitOfWork.CommitTransactionAsync(ct);
        return errorWhenSave is null ? Result.Deleted : errorWhenSave.Value;
    }
}