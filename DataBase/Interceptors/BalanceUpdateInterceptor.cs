using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using WarehouseManagement.Domain;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Interceptors;

public class BalanceUpdateInterceptor : ISaveChangesInterceptor
{
    public async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not WarehouseDbContext dbContext)
            return result;

        List<ReceiptResource>? changedReceipts = GetChangedResources<ReceiptResource>(dbContext);

        if (changedReceipts.Count > 0 && changedReceipts is not null)
        {
            await ProcessResourceChanges(dbContext, changedReceipts
                    .Select(r => (r.ResourceId, r.UnitOfMeasureId))
                    .Distinct()
                    .ToList(), cancellationToken);

            return result;
        }

        List<ShipmentResource>? changedShipments = GetChangedResources<ShipmentResource>(dbContext);
        if (changedShipments.Count > 0 && changedShipments is not null)
        {
            await ProcessResourceChanges(dbContext, changedShipments
                .Select(s => (s.ResourceId, s.UnitOfMeasureId))
                .Distinct()
                .ToList(), cancellationToken);
        }

        return result;
    }

    private async Task ProcessResourceChanges(
        WarehouseDbContext dbContext,
        List<(int ResourceId, int UnitOfMeasureId)> resourceChanges,
        CancellationToken cancellationToken)
    {
        foreach (var (resourceId, unitOfMeasureId) in resourceChanges)
        {
            await UpdateBalanceAsync(dbContext, resourceId, unitOfMeasureId, cancellationToken);
        }
    }

    private List<TEntity> GetChangedResources<TEntity>(WarehouseDbContext dbContext)
        where TEntity : class, IEntity
    {
        return dbContext.ChangeTracker.Entries<TEntity>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Select(x => x.Entity)
            .Distinct()
            .ToList();
    }

    private async Task UpdateBalanceAsync(
        WarehouseDbContext dbContext,
        int resourceId,
        int unitOfMeasureId,
        CancellationToken cancellationToken)
    {
        // Вычисляем общее количество для конкретной пары ResourceId + UnitOfMeasureId
        var totalReceipts = await dbContext.ReceiptResources
            .Where(r => r.ResourceId == resourceId && r.UnitOfMeasureId == unitOfMeasureId)
            .SumAsync(r => r.Quantity, cancellationToken);

        var totalShipments = await dbContext.ShipmentResources
            .Where(s => s.ResourceId == resourceId && s.UnitOfMeasureId == unitOfMeasureId)
            .SumAsync(s => (int?)s.Quantity, cancellationToken) ?? 0;

        var newBalance = totalReceipts - totalShipments;

        var balance = await dbContext.Balances
            .FirstOrDefaultAsync(b =>
                b.ResourceId == resourceId &&
                b.UnitOfMeasureId == unitOfMeasureId,
                cancellationToken);

        if (balance is null)
        {
            dbContext.Balances.Add(new Balance
            {
                ResourceId = resourceId,
                UnitOfMeasureId = unitOfMeasureId,
                Quantity = newBalance
            });
        }
        else
        {
            balance.Quantity = newBalance;
        }
    }
}
