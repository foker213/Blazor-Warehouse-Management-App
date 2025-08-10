using ErrorOr;
using WarehouseManagement.Domain;

namespace WarehouseManagement.Application;

public interface IRepository<T> where T : class, IEntity
{
    Task<List<T>> GetAllAsync(CancellationToken ct = default);
    Task<T?> GetByAsync(int id, CancellationToken ct = default);
    Task CreateAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(T entity, CancellationToken ct = default);
}