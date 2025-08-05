using WarehouseManagement.Application.IRepositories;
using WarehouseManagement.Application.IServices;

namespace WarehouseManagement.Application.Services;

internal sealed class ReceiptResourceService : IReceiptResourceService
{
    private readonly IReceiptResourceRepository _resourceRepository;

    public ReceiptResourceService(IReceiptResourceRepository resourceRepository)
    {
        _resourceRepository = resourceRepository;
    }

    public async Task<int> GetTotalQuantity(int resourceId, int unitId, CancellationToken ct = default)
        => await _resourceRepository.GetTotalQuantity(resourceId, unitId, ct);
}
