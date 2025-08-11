using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Application.IRepositories;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Repositories;

internal sealed class ResourceRepository(WarehouseDbContext db) : Repository<Resource>(db), IResourceRepository
{
    protected override IQueryable<Resource> GetQuery(bool isTracked = false)
    {
        if (isTracked)
            return DbSet.AsQueryable()
                .Include(x => x.ReceiptResources)
                .Include(x => x.ShipmentResources);
        else
            return DbSet.AsNoTracking()
                .Include(x => x.ReceiptResources)
                .Include(x => x.ShipmentResources);
    }

    public async Task ChangeStateAsync(Resource resource, CancellationToken ct = default)
    {
        DbSet.Entry(resource).Property(x => x.State).IsModified = true;
        await _db.SaveChangesAsync();;
    }

    public async Task<Resource?> GetByName(string name, CancellationToken ct = default)
    {
        IQueryable<Resource> query = GetQuery();
        return await query.Where(x => x.Name.ToLower() == name.ToLower()).FirstOrDefaultAsync(ct);
    }
}