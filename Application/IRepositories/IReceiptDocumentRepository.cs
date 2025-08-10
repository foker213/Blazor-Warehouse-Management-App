using WarehouseManagement.Contracts;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Application.IRepositories;

public interface IReceiptDocumentRepository : IRepository<ReceiptDocument>
{
    Task<List<ReceiptDocument>> FilterAsync(FilterDto filter, CancellationToken ct = default);
    Task<ReceiptDocument?> GetByNumberAsync(string number, CancellationToken ct = default);
    void Add(ReceiptDocument receiptDocument);
    void Update(ReceiptDocument receiptDocument);
    void Remove(ReceiptDocument receiptDocument);
}