using WarehouseManagement.Application.IRepositories;
using WarehouseManagement.Contracts;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Repositories;

internal sealed class ReceiptDocumentRepository(WarehouseDbContext db) :
    Repository<ReceiptDocument>(db),
    IReceiptDocumentRepository
{
    public Task<List<Balance>> FilterAsync(FilterDto filter, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}