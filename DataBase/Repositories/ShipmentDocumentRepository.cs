using WarehouseManagement.Contracts;
using WarehouseManagement.Domain.Models;
using WarehouseManagement.Domain.Repositories;

namespace WarehouseManagement.DataBase.Repositories;

internal sealed class ShipmentDocumentRepository(WarehouseDbContext db) :
    Repository<ShipmentDocument>(db),
    IShipmentDocumentRepository<FilterDto>
{
    public Task ChangeStatusAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Balance>> FilterAsync(FilterDto filter)
    {
        throw new NotImplementedException();
    }
}