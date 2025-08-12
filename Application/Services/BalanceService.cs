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
    public BalanceService(IBalanceRepository balanceRepository)
    {
        _balanceRepository = balanceRepository;
    }

    public async Task<List<BalanceDto>> FilterAsync(FilterDto filter, CancellationToken ct)
    {
        List<Balance> items = await _balanceRepository.FilterAsync(filter, ct);

        return items.Adapt<List<BalanceDto>>();
    }

    public async Task<List<BalanceDto>> GetAll(CancellationToken ct)
    {
        List<Balance> items = await _balanceRepository.GetAllAsync(ct);

        return items.Adapt<List<BalanceDto>>();
    }

    public async Task<ErrorOr<List<Balance>>> ProcessBalancesAsync(ReceiptDocument document, List<ReceiptResourceCreateDto> resources, CancellationToken ct)
    {
        var balanceChanges = CalculateBalanceChangesForCreation(resources);
        return await ApplyBalanceChanges(balanceChanges, ct);
    }

    public async Task<ErrorOr<List<Balance>>> ProcessBalanceUpdatesAsync(ReceiptDocument document, List<ReceiptResource> existingResources, List<ReceiptResource> deletedResources, CancellationToken ct)
    {
        var balanceChanges = CalculateBalanceChangesForUpdate(existingResources, deletedResources);
        return await ApplyBalanceChanges(balanceChanges, ct);
    }

    public void Add(List<Balance> balances)
    {
        foreach (Balance balance in balances)
        {
            if (balance.CheckExistInDb())
                _balanceRepository.Update(balance);
            else
                _balanceRepository.Add(balance);
        }
    }

    public void Update(List<Balance> balances)
    {
        foreach (Balance balance in balances)
        {
            if (balance.Quantity == 0)
                _balanceRepository.Remove(balance);
            else if (balance.Id == 0)
                _balanceRepository.Add(balance);
            else
                _balanceRepository.Update(balance);
        }
    }

    public void Remove(List<Balance> balances)
    {
        foreach (Balance balance in balances)
        {
            if (balance.Quantity == 0)
                _balanceRepository.Remove(balance);
            else
                _balanceRepository.Update(balance);
        }
    }

    private Dictionary<(int ResourceId, int UnitId), int> CalculateBalanceChangesForCreation(
        List<ReceiptResourceCreateDto> resources)
    {
        Dictionary<(int ResourceId, int UnitId), int> balanceChanges = new();

        foreach (ReceiptResourceCreateDto resource in resources)
        {
            var key = (resource.ResourceId, resource.UnitOfMeasureId);
            balanceChanges.TryGetValue(key, out int currentChange);
            balanceChanges[key] = currentChange + resource.Quantity;
        }

        return balanceChanges;
    }

    private Dictionary<(int ResourceId, int UnitId), int> CalculateBalanceChangesForUpdate(
        List<ReceiptResource> existingResources,
        List<ReceiptResource> deletedResources)
    {
        Dictionary<(int ResourceId, int UnitId), int> balanceChanges = new();

        foreach (ReceiptResource resource in existingResources)
        {
            var key = (resource.ResourceId, resource.UnitOfMeasureId);
            balanceChanges.TryGetValue(key, out var currentChange);
            balanceChanges[key] = currentChange + resource.Quantity;
        }

        foreach (ReceiptResource resource in deletedResources)
        {
            var key = (resource.ResourceId, resource.UnitOfMeasureId);
            balanceChanges.TryGetValue(key, out var currentChange);
            balanceChanges[key] = currentChange - resource.Quantity;
        }

        return balanceChanges;
    }

    private async Task<ErrorOr<List<Balance>>> ApplyBalanceChanges(
        Dictionary<(int ResourceId, int UnitId), int> balanceChanges,
        CancellationToken ct)
    {
        List<Balance> balances = new();

        foreach (var change in balanceChanges)
        {
            ErrorOr<Balance> balanceResult = await ApplySingleBalanceChange(
                change.Key.ResourceId,
                change.Key.UnitId,
                change.Value,
                ct);

            if (balanceResult.IsError) return balanceResult.Errors;
            balances.Add(balanceResult.Value);
        }

        return balances;
    }

    private async Task<ErrorOr<Balance>> ApplySingleBalanceChange(
        int resourceId,
        int unitId,
        int quantityDelta,
        CancellationToken ct)
    {
        Balance? balance = await _balanceRepository.GetByIdsAsync(resourceId, unitId, ct);

        if (balance is not null)
        {
            var result = quantityDelta >= 0
                ? balance.Increase(quantityDelta)
                : balance.Decrease(-quantityDelta);

            if (result.IsError) 
                return result.Errors;

            return balance;
        }

        if (quantityDelta >= 0)
        {
            var createResult = Balance.Create(resourceId, unitId, quantityDelta);
            if (createResult.IsError) 
                return createResult.Errors;

            return createResult.Value;
        }

        return Error.NotFound(
            "BalanceNotFound",
            $"Не найден баланс для ResourceId: {resourceId}, UnitId: {unitId}");
    }
}
