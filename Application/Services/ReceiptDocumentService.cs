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
    private readonly IReceiptResourceRepository _resourceRepository;
    private readonly IBalanceRepository _balanceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReceiptDocumentService(
        IReceiptDocumentRepository documentRepository, 
        IReceiptResourceRepository resourceRepository, 
        IBalanceRepository balanceRepository, 
        IUnitOfWork unitOfWork)
    {
        _documentRepository = documentRepository;
        _resourceRepository = resourceRepository;
        _balanceRepository = balanceRepository;
        _unitOfWork = unitOfWork;
    }

    private static readonly SemaphoreSlim _balanceLock = new(1, 1);

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
        await _balanceLock.WaitAsync(ct);

        try
        {
            Error? error = await ValidateReceipt(receipt, ct);
            if (error is not null)
                return ErrorOr<Created>.From(new List<Error> { error.Value });

            if (receipt.ReceiptResources is null || receipt.ReceiptResources.Count == 0)
            {
                ErrorOr<Created> result = await _documentRepository.CreateAsync(receipt.Adapt<ReceiptDocument>(), ct);

                if (result.IsError)
                    return result.FirstError;

                return result;
            }

            await using DbTransaction transaction = await _unitOfWork.BeginTransactionAsync(ct);

            _documentRepository.Add(receipt.Adapt<ReceiptDocument>());

            foreach (ReceiptResourceDto receiptResource in receipt.ReceiptResources)
            {
                int totalQuantity = await _resourceRepository.GetTotalQuantity(receiptResource.Resource.Id, receiptResource.Unit.Id, ct);

                Balance? balance = await _balanceRepository.GetByResourceIdAndUnitId(receiptResource.Resource.Id, receiptResource.Unit.Id, ct);

                if (balance is null)
                {
                    balance = new()
                    {
                        Quantity = totalQuantity,
                        ResourceId = receiptResource.Resource.Id,
                        UnitOfMeasureId = receiptResource.Unit.Id,
                    };

                    _balanceRepository.Add(balance);
                }
                else
                {
                    if (balance.Quantity != totalQuantity)
                    {
                        balance.Quantity = totalQuantity;
                        _balanceRepository.Update(balance);
                    }
                }
            }

            await _unitOfWork.CommitTransactionAsync(ct);
            return new Created();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            return Error.Failure("CreateFailed", "Ошибка при сохранении записи");
        }
        finally
        {
            _balanceLock.Release();
        }
    }

    public async Task<ErrorOr<Updated>> UpdateAsync(ReceiptDocumentDto receipt, CancellationToken ct = default)
    {
        await _balanceLock.WaitAsync(ct);

        try
        {
            Error? error = await ValidateReceipt(receipt, ct);
            if (error is not null && error.Value.Code != "NumberExist")
                return ErrorOr<Updated>.From(new List<Error> { error.Value });

            ReceiptDocument? existingDocument = await _documentRepository.GetBy(receipt.Id, ct);
            if (existingDocument is null)
                return Error.NotFound("DocumentNotFound", "Документ не найден");

            await using DbTransaction transaction = await _unitOfWork.BeginTransactionAsync(ct);

            List<ReceiptResource> oldResources = existingDocument.ReceiptResources?.ToList() ?? new List<ReceiptResource>();
            List<ReceiptResourceDto> newResources = receipt.ReceiptResources?.ToList() ?? new List<ReceiptResourceDto>();

            var resourcesToRemove = oldResources
                .Where(old => !newResources.Any(newRes =>
                    newRes.Resource.Id == old.ResourceId &&
                    newRes.Unit.Id == old.UnitOfMeasureId))
                .ToList();

            // Группируем для проверки остатка на балансе
            var groupBalance = resourcesToRemove
                .GroupBy(r => new { r.ResourceId, r.UnitOfMeasureId })
                .Select(g => new
                {
                    Key = g.Key,
                    TotalQuantityToRemove = g.Sum(x => x.Quantity)
                });

            foreach (var group in groupBalance)
            {
                int totalQuantity = await _resourceRepository.GetTotalQuantity(
                    group.Key.ResourceId,
                    group.Key.UnitOfMeasureId,
                    ct);

                if (totalQuantity - group.TotalQuantityToRemove < 0)
                {
                    return Error.Conflict("NegativeBalance", "Удаление ресурса приведёт к отрицательному балансу.");
                }
            }

            await _unitOfWork.CommitTransactionAsync(ct);
            return new Updated();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            return Error.Failure("CreateFailed", "Ошибка при сохранении записи");
        }
        finally
        {
            _balanceLock.Release();
        }
    }

    public Task<ErrorOr<Deleted>> DeleteAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
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

        if (response is not null)
            return Error.Conflict("NumberExist", "Поставка с таким номером уже существует");

        return default;
    }
}
