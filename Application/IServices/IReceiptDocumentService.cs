using ErrorOr;
using WarehouseManagement.Contracts;
using WarehouseManagement.Contracts.Receipt;

namespace WarehouseManagement.Application.IServices;

public interface IReceiptDocumentService
{
    Task<List<ReceiptDocumentDto>> GetAll(CancellationToken ct);
    Task<ErrorOr<ReceiptDocumentUpdateDto>> GetBy(int id, CancellationToken ct);
    Task<ErrorOr<Created>> CreateAsync(ReceiptDocumentCreateDto entity, CancellationToken ct);
    Task<ErrorOr<Updated>> UpdateAsync(ReceiptDocumentUpdateDto entity, CancellationToken ct);
    Task<ErrorOr<Deleted>> DeleteAsync(int id, CancellationToken ct);
    Task<List<ReceiptDocumentDto>> FilterAsync(FilterDto filter, CancellationToken ct);
}
