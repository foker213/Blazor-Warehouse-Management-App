using ErrorOr;
using WarehouseManagement.Domain;

namespace WarehouseManagement.Application;

public interface IRepository<T> where T : class, IEntity
{
    Task<List<T>> GetAll(CancellationToken ct = default);
    Task<T?> GetBy(int id, CancellationToken ct = default);
    Task<ErrorOr<Created>> CreateAsync(T entity, CancellationToken ct = default);
    Task<ErrorOr<Updated>> UpdateAsync(T entity, CancellationToken ct = default);
    Task<ErrorOr<Deleted>> DeleteAsync(T entity, CancellationToken ct = default);
}