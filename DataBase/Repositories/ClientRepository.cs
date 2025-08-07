using ErrorOr;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Application.IRepositories;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Repositories;

internal sealed class ClientRepository(WarehouseDbContext db) : Repository<Client>(db), IClientRepository
{
    protected override IQueryable<Client> GetQuery()
    {
        return DbSet.AsNoTracking()
            .Include(x => x.ShipmentDocument);
    }

    public async Task<ErrorOr<Updated>> ChangeStateAsync(Client client, CancellationToken ct = default)
    {
        DbSet.Entry(client).Property(x => x.State).IsModified = true;
        await db.SaveChangesAsync();

        return new Updated();
    }

    public override async Task<ErrorOr<Deleted>> DeleteAsync(Client client, CancellationToken ct = default)
    {
        DbSet.Remove(client);

        try
        {
            await db.SaveChangesAsync(ct);
            return new Deleted();
        }
        catch
        {
            return Error.Failure("DeleteFailed", "Ошибка при удалении клиента");
        }
    }

    public async Task<Client?> GetByName(string name, CancellationToken ct = default)
    {
        IQueryable<Client> query = GetQuery();
        return await query.Where(x => x.Name.ToLower() == name.ToLower()).FirstOrDefaultAsync(ct);
    }
}
