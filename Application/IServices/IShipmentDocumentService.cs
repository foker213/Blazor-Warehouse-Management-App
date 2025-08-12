using ErrorOr;
using WarehouseManagement.Contracts;
using WarehouseManagement.Contracts.Shipment;

namespace WarehouseManagement.Application.IServices;

public interface IShipmentDocumentService
{
    Task<List<ShipmentDocumentDto>> GetAll(CancellationToken ct);
    Task<ErrorOr<ShipmentDocumentUpdateDto>> GetBy(int id, CancellationToken ct);
    Task<ErrorOr<Created>> CreateAsync(ShipmentDocumentCreateDto shipment, CancellationToken ct);
    Task<ErrorOr<Updated>> UpdateAsync(ShipmentDocumentUpdateDto shipment, CancellationToken ct);
    Task<ErrorOr<Deleted>> DeleteAsync(int id, CancellationToken ct);
    Task<ErrorOr<Updated>> ChangeStatusAsync(ShipmentDocumentUpdateDto shipment, CancellationToken ct);
    Task<List<ShipmentDocumentDto>> FilterAsync(FilterDto filter, CancellationToken ct);
} 