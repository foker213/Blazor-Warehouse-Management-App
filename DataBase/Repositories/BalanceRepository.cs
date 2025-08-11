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

    private IQueryable<Balance> GetQueryWithoutIncludes()
    {
        return DbSet.AsNoTracking();
    }

    public async Task<List<Balance>> FilterAsync(FilterDto filter, CancellationToken ct = default)
    {
        IQueryable<Balance> query = GetQuery();

        if (filter.Resources is not null && filter.Resources.Count > 0)
            query = query.Where(x => filter.Resources.Any(s => s == x.Resource.Name));

        if (filter.UnitsOfMeasure is not null && filter.UnitsOfMeasure.Count > 0)
            query = query.Where(x => filter.UnitsOfMeasure.Any(s => s == x.UnitOfMeasure.Name));

        return await query.ToListAsync(ct);
    }

    public async Task<Balance?> GetByIdsAsync(int resourceId, int unitId, CancellationToken ct = default)
    {
        IQueryable<Balance> query = GetQueryWithoutIncludes();
        return await query.FirstOrDefaultAsync(x => x.ResourceId == resourceId && x.UnitOfMeasureId == unitId);
    }

    public void Add(Balance document)
        => DbSet.Add(document);

    public void Update(Balance document)
        => DbSet.Update(document);

    public void Remove(Balance document)
        => DbSet.Remove(document);
}
