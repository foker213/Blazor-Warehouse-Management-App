using ErrorOr;
using Mapster;
using System.Data.Common;
using WarehouseManagement.Application.IRepositories;
using WarehouseManagement.Application.IServices;
using WarehouseManagement.Contracts;
using WarehouseManagement.Contracts.Receipt;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Application.Services;

internal sealed class ReceiptDocumentService : IReceiptDocumentService
{
    private readonly IReceiptDocumentRepository _documentRepository;
    private readonly IBalanceService _balanceService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISynchronizationService _syncService;

    public ReceiptDocumentService(
        IReceiptDocumentRepository documentRepository, 
        IBalanceService balanceService, 
        IUnitOfWork unitOfWork,
        ISynchronizationService syncService)
    {
        _documentRepository = documentRepository;
        _balanceService = balanceService;
        _unitOfWork = unitOfWork;
        _syncService = syncService;
    }

    public async Task<List<ReceiptDocumentDto>> GetAll(CancellationToken ct = default)
    {
        List<ReceiptDocument> items = await _documentRepository.GetAll(ct);

        return items.Adapt<List<ReceiptDocumentDto>>();
    }

    public async Task<ErrorOr<ReceiptDocumentDto>> GetBy(int id, CancellationToken ct = default)
    {
        ReceiptDocument? client = await _documentRepository.GetBy(id, ct);

        if (client is null)
            return ErrorOr<ReceiptDocumentDto>.From(new List<Error> { Error.NotFound() });

        return client.Adapt<ReceiptDocumentDto>();
    }

    public async Task<ErrorOr<Created>> CreateAsync(ReceiptDocumentDto receipt, CancellationToken ct = default)
    {
        Error? error = await ValidateReceipt(receipt, ct);
        if (error is not null)
            return error.Value;

        if (!receipt.HasResources())
        {
            return await _documentRepository.CreateAsync(receipt.Adapt<ReceiptDocument>(), ct);
        }

        List<IDisposable> locks = await GetInLineAsync(receipt.ReceiptResources);

        await using DbTransaction transaction = await _unitOfWork.BeginTransactionAsync(ct);

        _documentRepository.Add(receipt.Adapt<ReceiptDocument>());

        List<ReceiptResourceDto> updatedResources = receipt.ReceiptResources!.Where(x => x.Quantity > 0).ToList();

        await _balanceService.UpdateBalances(updatedResources, ct);

        Error? errorWhenSave = await _unitOfWork.CommitTransactionAsync(ct);

        foreach (IDisposable currentLock in locks)
            currentLock.Dispose();

        if (errorWhenSave is not null)
            return errorWhenSave.Value;

        return new Created();
    }

    public async Task<ErrorOr<Updated>> UpdateAsync(ReceiptDocumentDto receipt, CancellationToken ct = default)
    {
        Error? error = await ValidateReceipt(receipt, ct);
        if (error is not null && error.Value.Code != "NumberExist")
            return error.Value;

        ReceiptDocument? existingDocument = await _documentRepository.GetBy(receipt.Id, ct);
        if (existingDocument is null)
            return Error.NotFound("DocumentNotFound", "Документ не найден");

        List<IDisposable> locks = await GetInLineAsync(receipt.ReceiptResources);

        await using DbTransaction transaction = await _unitOfWork.BeginTransactionAsync(ct);

        List<ReceiptResource> oldResources = existingDocument.ReceiptResources?.ToList() ?? new List<ReceiptResource>();
        List<ReceiptResourceDto> newResources = receipt.ReceiptResources?.ToList() ?? new List<ReceiptResourceDto>();

        ErrorOr<Success> removedResourcesResult = await _balanceService.HandleRemovedResourcesAsync(
            oldResources, newResources, ct);

        if (removedResourcesResult.IsError)
            return removedResourcesResult.FirstError;

        _documentRepository.Update(receipt.Adapt<ReceiptDocument>());

        newResources = newResources.Where(x => x.Quantity > 0).ToList();

        if (newResources.Any())
            await _balanceService.UpdateBalances(newResources, ct);

        Error? errorWhenSave = await _unitOfWork.CommitTransactionAsync(ct);

        foreach (IDisposable currentLock in locks)
            currentLock.Dispose();

        if (errorWhenSave is not null)
            return errorWhenSave.Value;

        return new Updated();
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(int id, CancellationToken ct)
    {
        ReceiptDocument? document = await _documentRepository.GetBy(id, ct);
        if (document is null)
            return Error.NotFound("DocumentNotFound", "Документ не найден");

        if (document.ReceiptResources is null || document.ReceiptResources.Count == 0)
        {
            return await _documentRepository.DeleteAsync(document, ct);
        }

        List<IDisposable> locks = await GetInLineAsync(document.ReceiptResources.Adapt<List<ReceiptResourceDto>>());

        await using DbTransaction transaction = await _unitOfWork.BeginTransactionAsync(ct);

        await _balanceService.UpdateBalances(document.ReceiptResources.Adapt<List<ReceiptResourceDto>>(), ct);

        _documentRepository.Remove(document);

        Error? errorWhenSave = await _unitOfWork.CommitTransactionAsync(ct);

        foreach (IDisposable currentLock in locks)
            currentLock.Dispose();

        if (errorWhenSave is not null)
            return errorWhenSave.Value;

        return new Deleted();
    }

    public async Task<List<ReceiptDocumentDto>> FilterAsync(FilterDto filter, CancellationToken ct = default)
    {
        List<ReceiptDocument> items = await _documentRepository.FilterAsync(filter, ct);

        return items.Adapt<List<ReceiptDocumentDto>>();
    }

    private async Task<Error?> ValidateReceipt(ReceiptDocumentDto receipt, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(receipt.Number))
            return Error.Validation("NumberNull", "Поле номера должно быть заполнено");

        if(receipt.ReceiptResources is not null && receipt.ReceiptResources.Count > 0)
        {
            foreach (ReceiptResourceDto resource in receipt.ReceiptResources)
            {
                if (resource.Quantity < 0)
                {
                    return Error.Validation("NegativeBalance", "Количество ресурсов не может быть отрицательным");
                }
            }
        }

        ReceiptDocument? response = await _documentRepository.GetByNumber(receipt.Number, ct);

        if (response is not null && receipt.Id != response.Id)
            return Error.Conflict("NumberExist", "Поставка с таким номером уже существует");

        return default;
    }

    private async Task<List<IDisposable>> GetInLineAsync(List<ReceiptResourceDto>? receiptResources)
    {
        receiptResources = receiptResources ?? new();

        var resourceLocks = receiptResources
            .Where(x => x.Quantity > 0)
            .Select(x => (x.Resource.Id, x.Unit.Id))
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
}