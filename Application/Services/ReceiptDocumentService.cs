using ErrorOr;
using WarehouseManagement.Application.IRepositories;
using WarehouseManagement.Application.IServices;
using WarehouseManagement.Contracts;
using WarehouseManagement.Contracts.Receipt;

namespace WarehouseManagement.Application.Services;

internal sealed class ReceiptDocumentService : IReceiptDocumentService
{
    private readonly IReceiptDocumentRepository _documentRepository;

    public ReceiptDocumentService(IReceiptDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public Task<ErrorOr<Created>> CreateAsync(ReceiptDocumentDto entity, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Deleted>> DeleteAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<ReceiptDocumentDto>> FilterAsync(FilterDto filter, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<ReceiptDocumentDto>> GetAll(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<ReceiptDocumentDto>> GetBy(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Updated>> UpdateAsync(ReceiptDocumentDto entity, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
