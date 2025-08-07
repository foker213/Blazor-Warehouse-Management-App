namespace WarehouseManagement.Application.IServices;

internal interface ISynchronizationService
{
    Task<IDisposable> AcquireLockAsync(int resourceId, int unitId);
}
