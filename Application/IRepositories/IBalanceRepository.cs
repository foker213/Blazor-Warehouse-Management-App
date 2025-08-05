using WarehouseManagement.Contracts;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Application.IRepositories;

public interface IBalanceRepository : IRepository<Balance>
{
    Task<List<Balance>> FilterAsync(FilterDto filter, CancellationToken ct = default);
    void Add(Balance document);
    void Update(Balance document);
    void Remove(Balance document);
    Task<Balance?> GetByResourceIdAndUnitId(int resourceId, int unitId, CancellationToken ct = default);
}
