using WarehouseManagement.Contracts;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Application.IRepositories;

public interface IReceiptDocumentRepository : IRepository<ReceiptDocument>
{
    Task<List<Balance>> FilterAsync(FilterDto filter, CancellationToken ct = default);
}
