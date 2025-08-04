using ErrorOr;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Application.IRepositories;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Repositories;

internal sealed class ClientRepository(WarehouseDbContext db) : Repository<Client>(db), IClientRepository
{
    public Task<ErrorOr<Updated>> ChangeStateAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
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
