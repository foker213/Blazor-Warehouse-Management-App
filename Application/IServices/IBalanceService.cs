using ErrorOr;
using WarehouseManagement.Contracts;
using WarehouseManagement.Contracts.Balance;
using WarehouseManagement.Contracts.Receipt;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Application.IServices;

public interface IBalanceService
{
    Task<List<BalanceDto>> GetAll(CancellationToken ct);
    Task<List<BalanceDto>> FilterAsync(FilterDto filter, CancellationToken ct);
    Task UpdateBalances(IEnumerable<ReceiptResourceDto> resources, CancellationToken ct);
    Task<ErrorOr<Success>> HandleRemovedResourcesAsync(
        IEnumerable<ReceiptResource> oldResources,
        IEnumerable<ReceiptResourceDto> newResources,
        CancellationToken ct);
}
