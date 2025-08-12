using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Application.IRepositories;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Repositories;

internal sealed class UnitOfMeasureRepository(WarehouseDbContext db) : Repository<UnitOfMeasure>(db), IUnitOfMeasureRepository
{
    protected override IQueryable<UnitOfMeasure> GetQuery(bool isTracked = false)
    {
        if(isTracked)
            return DbSet.AsQueryable()
                .Include(x => x.ReceiptResources)
                .Include(x => x.ShipmentResources);
        else
            return DbSet.AsNoTracking()
                .Include(x => x.ReceiptResources)
                .Include(x => x.ShipmentResources);
    }
    public async Task ChangeStateAsync(UnitOfMeasure unit, CancellationToken ct = default)
    {
        DbSet.Entry(unit).Property(x => x.State).IsModified = true;
        await _db.SaveChangesAsync();
    }

    public async Task<UnitOfMeasure?> GetByName(string name, CancellationToken ct = default)
    {
        IQueryable<UnitOfMeasure> query = GetQuery();
        return await query.Where(x => x.Name.ToLower() == name.ToLower()).FirstOrDefaultAsync(ct);
    }
}