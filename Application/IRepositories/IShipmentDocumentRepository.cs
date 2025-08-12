using ErrorOr;
using WarehouseManagement.Contracts;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Application.IRepositories;

public interface IShipmentDocumentRepository : IRepository<ShipmentDocument>
{
    Task<ErrorOr<Updated>> ChangeStatusAsync(int id, CancellationToken ct = default);
    Task<List<ShipmentDocument>> FilterAsync(FilterDto filter, CancellationToken ct = default);
    Task<ShipmentDocument?> GetByNumberAsync(string number, CancellationToken ct = default);
    void Add(ShipmentDocument receiptDocument);
    void Update(ShipmentDocument receiptDocument);
    void Remove(ShipmentDocument receiptDocument);
}