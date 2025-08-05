using WarehouseManagement.Contracts;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Application.IRepositories;

public interface IReceiptDocumentRepository : IRepository<ReceiptDocument>
{
    Task<List<ReceiptDocument>> FilterAsync(FilterDto filter, CancellationToken ct = default);
    Task<ReceiptDocument?> GetByNumber(string number, CancellationToken ct = default);
    void Add(ReceiptDocument document);
    void Update(ReceiptDocument document);
    void Remove(ReceiptDocument document);
}
