using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Application.IRepositories;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Repositories;

internal sealed class ClientRepository(WarehouseDbContext db) : Repository<Client>(db), IClientRepository
{
    protected override IQueryable<Client> GetQuery(bool isTracked = false)
    {
        if(isTracked)
            return DbSet.AsQueryable()
                .Include(x => x.ShipmentDocuments);
        else
            return DbSet.AsNoTracking()
                .Include(x => x.ShipmentDocuments);
    }

    public async Task ChangeStateAsync(Client client, CancellationToken ct = default)
    {
        DbSet.Entry(client).Property(x => x.State).IsModified = true;
        await _db.SaveChangesAsync();
    }

    public async Task<Client?> GetByName(string name, CancellationToken ct = default)
    {
        IQueryable<Client> query = GetQuery();
        return await query.Where(x => x.Name.ToLower() == name.ToLower()).FirstOrDefaultAsync(ct);
    }
}