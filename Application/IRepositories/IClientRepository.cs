using ErrorOr;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Application.IRepositories;

public interface IClientRepository : IRepository<Client>
{
    Task<ErrorOr<Updated>> ChangeStateAsync(int id, CancellationToken ct = default);
    Task<Client?> GetByName(string name, CancellationToken ct = default);
}