using ErrorOr;
using WarehouseManagement.Contracts;
using WarehouseManagement.Contracts.Shipment;

namespace WarehouseManagement.Application.IServices;

public interface IShipmentDocumentService
{
    Task<List<ShipmentDocumentDto>> GetAll(CancellationToken ct = default);
    Task<ErrorOr<ShipmentDocumentDto>> GetBy(int id, CancellationToken ct = default);
    Task<ErrorOr<Created>> CreateAsync(ShipmentDocumentDto entity, CancellationToken ct = default);
    Task<ErrorOr<Updated>> UpdateAsync(ShipmentDocumentDto entity, CancellationToken ct = default);
    Task<ErrorOr<Deleted>> DeleteAsync(int id, CancellationToken ct = default);
    Task<ErrorOr<Updated>> ChangeStatusAsync(int id, CancellationToken ct = default);
    Task<List<ShipmentDocumentDto>> FilterAsync(FilterDto filter, CancellationToken ct = default);
}