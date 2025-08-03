using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Domain.Repositories;

public interface IReceiptDocumentRepository<TFilter> : IRepository<ReceiptDocument>
    where TFilter : class 
{
    Task<List<Balance>> FilterAsync(TFilter filter);
}
