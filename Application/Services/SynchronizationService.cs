using WarehouseManagement.Application.IServices;

namespace WarehouseManagement.Application.Services;

internal sealed class SynchronizationService : ISynchronizationService
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public async Task<IDisposable> AcquireLockAsync()
    {
        await _semaphore.WaitAsync();
        return new ReleaseWrapper(_semaphore);
    }

    private class ReleaseWrapper : IDisposable
    {
        private readonly SemaphoreSlim _semaphore;

        public ReleaseWrapper(SemaphoreSlim semaphore) => _semaphore = semaphore;

        public void Dispose() => _semaphore.Release();
    }
}
