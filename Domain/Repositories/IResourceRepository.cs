using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Domain.Repositories;

public interface IResourceRepository : IRepository<Resource> 
{
    Task ChangeStateAsync(int id);
}
