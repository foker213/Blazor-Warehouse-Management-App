namespace WarehouseManagement.Application.IServices;

internal interface IReceiptResourceService
{
    Task<int> GetTotalQuantity(int resourceId, int unitId, CancellationToken ct = default);
}
