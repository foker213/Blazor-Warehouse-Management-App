using ErrorOr;
using WarehouseManagement.Contracts;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Application.IRepositories;

public interface IShipmentDocumentRepository : IRepository<ShipmentDocument>
{
    Task<ErrorOr<Updated>> ChangeStatusAsync(int id, CancellationToken ct = default);
    Task<List<Balance>> FilterAsync(FilterDto filter, CancellationToken ct = default);
}