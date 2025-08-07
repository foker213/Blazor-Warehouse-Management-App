using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Application.IRepositories;
using WarehouseManagement.Contracts;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Repositories;

internal sealed class BalanceRepository(WarehouseDbContext db) :
    Repository<Balance>(db),
    IBalanceRepository
{
    protected override IQueryable<Balance> GetQuery()
    {
        return DbSet.AsNoTracking()
            .Include(x => x.UnitOfMeasure)
            .Include(x => x.Resource);
    }

    public async Task<List<Balance>> FilterAsync(FilterDto filter, CancellationToken ct = default)
    {
        IQueryable<Balance> query = GetQuery();

        if (!string.IsNullOrEmpty(filter.Resource))
            query = query.Where(x => x.Resource.Name.ToLower() == filter.Resource);

        if (!string.IsNullOrEmpty(filter.UnitOfMeasure))
            query = query.Where(x => x.UnitOfMeasure.Name.ToLower() == filter.UnitOfMeasure);

        return await query.ToListAsync(ct);
    }

    public async Task<Balance?> GetByResourceIdAndUnitId(int resourceId, int unitId, CancellationToken ct = default)
    {
        IQueryable<Balance> query = GetQuery();
        return await query.Where(x => x.ResourceId == resourceId && x.UnitOfMeasureId == unitId).FirstOrDefaultAsync(ct);
    }

    public void Add(Balance document)
        => DbSet.Add(document);

    public void Update(Balance document)
        => DbSet.Update(document);

    public void Remove(Balance document)
        => DbSet.Remove(document);
}
