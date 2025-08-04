using ErrorOr;
using WarehouseManagement.Contracts;
using WarehouseManagement.Contracts.Receipt;

namespace WarehouseManagement.Application.IServices;

public interface IReceiptDocumentService
{
    Task<List<ReceiptDocumentDto>> GetAll(CancellationToken ct = default);
    Task<ErrorOr<ReceiptDocumentDto>> GetBy(int id, CancellationToken ct = default);
    Task<ErrorOr<Created>> CreateAsync(ReceiptDocumentDto entity, CancellationToken ct = default);
    Task<ErrorOr<Updated>> UpdateAsync(ReceiptDocumentDto entity, CancellationToken ct = default);
    Task<ErrorOr<Deleted>> DeleteAsync(int id, CancellationToken ct = default);
    Task<List<ReceiptDocumentDto>> FilterAsync(FilterDto filter, CancellationToken ct = default);
}
