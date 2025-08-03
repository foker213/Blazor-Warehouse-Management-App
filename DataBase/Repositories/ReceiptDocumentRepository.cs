using WarehouseManagement.Contracts;
using WarehouseManagement.Domain.Models;
using WarehouseManagement.Domain.Repositories;

namespace WarehouseManagement.DataBase.Repositories;

internal sealed class ReceiptDocumentRepository(WarehouseDbContext db) :
    Repository<ReceiptDocument>(db),
    IReceiptDocumentRepository<FilterDto>
{
    public Task<List<Balance>> FilterAsync(FilterDto filter)
    {
        throw new NotImplementedException();
    }
}