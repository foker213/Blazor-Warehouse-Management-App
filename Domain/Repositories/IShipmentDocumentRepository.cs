using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Domain.Repositories;

public interface IShipmentDocumentRepository<TFilter> : IRepository<ShipmentDocument>
    where TFilter : class 
{
    Task ChangeStatusAsync(int id);
    Task<List<Balance>> FilterAsync(TFilter filter);
}