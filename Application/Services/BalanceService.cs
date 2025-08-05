using ErrorOr;
using Mapster;
using WarehouseManagement.Application.IRepositories;
using WarehouseManagement.Application.IServices;
using WarehouseManagement.Contracts;
using WarehouseManagement.Contracts.Balance;
using WarehouseManagement.Contracts.Receipt;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Application.Services;

internal sealed class BalanceService : IBalanceService
{
    private readonly IBalanceRepository _balanceRepository;
    private readonly IReceiptResourceRepository _resourceService;
    public BalanceService(IBalanceRepository balanceRepository, IReceiptResourceRepository resourceRepository)
    {
        _balanceRepository = balanceRepository;
        _resourceService = resourceRepository;
    }

    public async Task<List<BalanceDto>> FilterAsync(FilterDto filter, CancellationToken ct)
    {
        List<Balance> items = await _balanceRepository.FilterAsync(filter, ct);

        return items.Adapt<List<BalanceDto>>();
    }

    public async Task<List<BalanceDto>> GetAll(CancellationToken ct)
    {
        List<Balance> items = await _balanceRepository.GetAll(ct);

        return items.Adapt<List<BalanceDto>>();
    }

    public async Task UpdateBalances(
    IEnumerable<ReceiptResourceDto> resources,
    CancellationToken ct)
    {
        foreach (var receiptResource in resources)
        {
            int totalQuantity = await _resourceService.GetTotalQuantity(
                receiptResource.Resource.Id,
                receiptResource.Unit.Id,
                ct);

            Balance? balance = await _balanceRepository.GetByResourceIdAndUnitId(
                receiptResource.Resource.Id,
                receiptResource.Unit.Id,
                ct);

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
            else if (balance.Quantity != totalQuantity)
            {
                balance.Quantity = totalQuantity;
                _balanceRepository.Update(balance);
            }
        }
    }

    public async Task<ErrorOr<Success>> HandleRemovedResourcesAsync(
        IEnumerable<ReceiptResource> oldResources,
        IEnumerable<ReceiptResourceDto> newResources,
        CancellationToken ct)
    {
        var removedResourcesGrouped = oldResources
            .Where(old => !newResources.Any(newRes =>
                newRes.Resource.Id == old.ResourceId &&
                newRes.Unit.Id == old.UnitOfMeasureId))
            .GroupBy(r => new { r.ResourceId, r.UnitOfMeasureId })
            .Select(g => new
            {
                g.Key.ResourceId,
                g.Key.UnitOfMeasureId,
                TotalQuantityToRemove = g.Sum(x => x.Quantity)
            })
            .ToList();

        foreach (var group in removedResourcesGrouped)
        {
            int totalQuantity = await _resourceService.GetTotalQuantity(
                group.ResourceId,
                group.UnitOfMeasureId,
                ct);

            if (totalQuantity - group.TotalQuantityToRemove < 0)
            {
                return Error.Conflict("NegativeBalance", "Удаление ресурса приведёт к отрицательному балансу.");
            }

            Balance? balance = await _balanceRepository.GetByResourceIdAndUnitId(
                group.ResourceId,
                group.UnitOfMeasureId,
                ct);

            if (balance is null)
            {
                balance = new()
                {
                    Quantity = totalQuantity,
                    ResourceId = group.ResourceId,
                    UnitOfMeasureId = group.UnitOfMeasureId,
                };
                _balanceRepository.Add(balance);
            }
            else if (totalQuantity - group.TotalQuantityToRemove == 0)
            {
                _balanceRepository.Remove(balance);
            }
            else if (balance.Quantity != totalQuantity)
            {
                balance.Quantity = totalQuantity;
                _balanceRepository.Update(balance);
            }
        }

        return new();
    }
}
