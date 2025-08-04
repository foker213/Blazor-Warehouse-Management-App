using ErrorOr;
using WarehouseManagement.Application.IRepositories;
using WarehouseManagement.Contracts;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Repositories;

internal sealed class ShipmentDocumentRepository(WarehouseDbContext db) :
    Repository<ShipmentDocument>(db),
    IShipmentDocumentRepository
{
    public Task<ErrorOr<Updated>> ChangeStatusAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<Balance>> FilterAsync(FilterDto filter, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}