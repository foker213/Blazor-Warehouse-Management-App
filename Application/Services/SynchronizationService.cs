using System.Collections.Concurrent;
using WarehouseManagement.Application.IServices;

namespace WarehouseManagement.Application.Services;

/// <summary>
/// Сервис для соблюдения потокобезопасности
/// </summary>
internal sealed class SynchronizationService : ISynchronizationService
{
    private static readonly ConcurrentDictionary<(int ResourceId, int UnitId), SemaphoreSlim> _locks = new();
    private static readonly SemaphoreSlim _dictionaryLock = new(1, 1);

    public async Task<IDisposable> AcquireLockAsync(int resourceId, int unitId)
    {
        SemaphoreSlim semaphore;

        await _dictionaryLock.WaitAsync();
        try
        {
            semaphore = _locks.GetOrAdd((resourceId, unitId), _ => new SemaphoreSlim(1, 1));
        }
        finally
        {
            _dictionaryLock.Release();
        }

        await semaphore.WaitAsync();
        return new ReleaseWrapper(semaphore, resourceId, unitId);
    }

    private class ReleaseWrapper : IDisposable
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly int _resourceId;
        private readonly int _unitId;

        public ReleaseWrapper(SemaphoreSlim semaphore, int resourceId, int unitId)
        {
            _semaphore = semaphore;
            _resourceId = resourceId;
            _unitId = unitId;
        }

        public void Dispose()
        {
            _semaphore.Release();

            if (_semaphore.CurrentCount == 1)
            {
                _dictionaryLock.Wait();
                try
                {
                    if (_semaphore.CurrentCount == 1)
                    {
                        _locks.TryRemove((_resourceId, _unitId), out _);
                    }
                }
                finally
                {
                    _dictionaryLock.Release();
                }
            }
        }
    }
}
