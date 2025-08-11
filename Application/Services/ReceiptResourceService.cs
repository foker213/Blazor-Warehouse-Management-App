using ErrorOr;
using WarehouseManagement.Application.IRepositories;
using WarehouseManagement.Application.IServices;
using WarehouseManagement.Contracts.Receipt;
using WarehouseManagement.Domain.Models;

internal sealed class ReceiptResourceService : IReceiptResourceService
{
    private readonly IBalanceRepository _balanceRepository;
    private readonly ISynchronizationService _syncService;

    public ReceiptResourceService(
        IBalanceRepository balanceRepository,
        ISynchronizationService syncService)
    {
        _balanceRepository = balanceRepository;
        _syncService = syncService;
    }

    public async Task<ErrorOr<List<Balance>>> ProcessResourcesAsync(
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
            return Error.Conflict("UnknownException", "Произошла непредвиденная ошибка");
        }
        finally
        {
            foreach (var lockObj in locks)
                lockObj.Dispose();
        }
    }

    public async Task<ErrorOr<List<Balance>>> ProcessResourceUpdatesAsync(
        ReceiptDocument document,
        List<ReceiptResource> existingResources,
        List<ReceiptResource> deletedResources,
        CancellationToken ct)
    {
        List<Balance> balances = new();
        List<IDisposable> locks = new();

        try
        {
            locks = await GetInLineAsync(document.ReceiptResources.ToList());

            // Process existing resources
            foreach (var resource in existingResources)
            {
                var balanceResult = await UpdateBalanceForResource(resource, ct);
                if (balanceResult.IsError) return balanceResult.Errors;
                balances.Add(balanceResult.Value);
            }

            // Process deleted resources
            foreach (var resource in deletedResources)
            {
                var balanceResult = await DecreaseBalanceForResource(resource, ct);
                if (balanceResult.IsError) return balanceResult.Errors;
                balances.Add(balanceResult.Value);
            }

            return balances;
        }
        catch
        {
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
            var result = balance.Increase(resourceDto.Quantity);
            if (result.IsError) return result.Errors;
            return balance;
        }

        return Balance.Create(resourceDto.ResourceId, resourceDto.UnitOfMeasureId, resourceDto.Quantity);
    }

    private async Task<ErrorOr<Balance>> UpdateBalanceForResource(
        ReceiptResource resource,
        CancellationToken ct)
    {
        Balance? balance = await _balanceRepository
            .GetByIdsAsync(resource.ResourceId, resource.UnitOfMeasureId, ct);

        if (balance is not null)
        {
            int difference = resource.RecalculateDifference();
            var result = balance.ChangeQueantity(balance.Quantity - difference);
            if (result.IsError) return result.Errors;
            return balance;
        }

        return Balance.Create(resource.ResourceId, resource.UnitOfMeasureId, resource.Quantity);
    }

    private async Task<ErrorOr<Balance>> DecreaseBalanceForResource(
        ReceiptResource resource,
        CancellationToken ct)
    {
        Balance? balance = await _balanceRepository
            .GetByIdsAsync(resource.ResourceId, resource.UnitOfMeasureId, ct);

        if (balance is not null)
        {
            var result = balance.Decrease(resource.Quantity);
            if (result.IsError) return result.Errors;
            return balance;
        }

        return Error.NotFound("BalanceNotFound", "Баланс для ресурса не найден");
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

    public async Task<ErrorOr<List<Balance>>> ProcessResourceDeletionsAsync(List<ReceiptResource> deletedResources, CancellationToken ct)
    {
        List<Balance> balances = new();
        List<IDisposable> locks = new();

        try
        {
            locks = await GetInLineAsync(deletedResources);

            foreach (var resource in deletedResources)
            {
                var balanceResult = await DecreaseBalanceForResource(resource, ct);
                if (balanceResult.IsError) return balanceResult.Errors;

                balances.Add(balanceResult.Value);
            }

            return balances;
        }
        catch
        {
            return Error.Conflict("UnknownException", "Произошла непредвиденная ошибка");
        }
        finally
        {
            foreach (var lockObj in locks)
                lockObj.Dispose();
        }
    }
}