using ErrorOr;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Application.IRepositories;

public interface IResourceRepository : IRepository<Resource> 
{
    Task<ErrorOr<Updated>> ChangeStateAsync(int id, CancellationToken ct = default);
    Task<Resource?> GetByName(string name, CancellationToken ct = default);
}
