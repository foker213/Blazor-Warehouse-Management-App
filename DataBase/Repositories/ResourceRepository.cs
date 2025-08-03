using WarehouseManagement.Domain.Models;
using WarehouseManagement.Domain.Repositories;

namespace WarehouseManagement.DataBase.Repositories;

internal sealed class ResourceRepository(WarehouseDbContext db) : Repository<Resource>(db), IResourceRepository
{
    public Task ChangeStateAsync(int id)
    {
        throw new NotImplementedException();
    }
}