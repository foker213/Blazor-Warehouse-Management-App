using WarehouseManagement.Domain.Models;
using WarehouseManagement.Domain.Repositories;

namespace WarehouseManagement.DataBase.Repositories;

internal sealed class ClientRepository(WarehouseDbContext db) : Repository<Client>(db), IClientRepository
{
    public Task ChangeStatusAsync(int id)
    {
        throw new NotImplementedException();
    }
}
