using WarehouseManagement.Contracts;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Application.IRepositories;

public interface IBalanceRepository : IRepository<Balance>
{
    Task<List<Balance>> FilterAsync(FilterDto filter, CancellationToken ct = default);
    Task<Balance?> GetByIdsAsync(int resourceId, int unitId, CancellationToken ct = default);
    void Add(Balance balance);
    void Update(Balance balance);
    void Remove(Balance balance);
}
