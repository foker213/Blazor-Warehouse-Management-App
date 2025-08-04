using WarehouseManagement.Contracts;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Application.IRepositories;

public interface IBalanceRepository : IRepository<Balance>
{
    Task<List<Balance>> FilterAsync(FilterDto filter, CancellationToken ct = default);
}
