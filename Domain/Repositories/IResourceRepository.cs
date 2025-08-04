using ErrorOr;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Domain.Repositories;

public interface IResourceRepository : IRepository<Resource> 
{
    Task<ErrorOr<Updated>> ChangeStateAsync(int id, CancellationToken ct = default);
}
