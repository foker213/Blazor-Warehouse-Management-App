using ErrorOr;
using WarehouseManagement.Application.IRepositories;
using WarehouseManagement.Application.IServices;
using WarehouseManagement.Contracts;
using WarehouseManagement.Contracts.Shipment;

namespace WarehouseManagement.Application.Services;

internal sealed class ShipmentDocumentService : IShipmentDocumentService
{
    private readonly IShipmentDocumentRepository _documentRepository;

    public ShipmentDocumentService(IShipmentDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public Task<ErrorOr<Updated>> ChangeStatusAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Created>> CreateAsync(ShipmentDocumentDto entity, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Deleted>> DeleteAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<ShipmentDocumentDto>> FilterAsync(FilterDto filter, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<ShipmentDocumentDto>> GetAll(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<ShipmentDocumentDto>> GetBy(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Updated>> UpdateAsync(ShipmentDocumentDto entity, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
