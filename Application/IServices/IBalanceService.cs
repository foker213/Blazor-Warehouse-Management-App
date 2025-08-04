using WarehouseManagement.Contracts;
using WarehouseManagement.Contracts.Balance;

namespace WarehouseManagement.Application.IServices;

public interface IBalanceService
{
    Task<List<BalanceDto>> GetAll(CancellationToken ct);
    Task<List<BalanceDto>> FilterAsync(FilterDto filter, CancellationToken ct);
}
