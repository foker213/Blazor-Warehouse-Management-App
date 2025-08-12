using ErrorOr;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Application.IRepositories;

public interface IResourceRepository : IRepository<Resource> 
{
    Task ChangeStateAsync(Resource resource, CancellationToken ct = default);
    Task<Resource?> GetByName(string name, CancellationToken ct = default);
}
