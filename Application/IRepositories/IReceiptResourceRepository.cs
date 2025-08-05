namespace WarehouseManagement.Application.IRepositories;

public interface IReceiptResourceRepository
{
    Task<int> GetTotalQuantity(int resourceId, int unitId, CancellationToken ct = default);
}
