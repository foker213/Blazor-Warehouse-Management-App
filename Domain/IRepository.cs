
using ErrorOr;

namespace WarehouseManagement.Domain;

public interface IRepository<T> where T : class, IEntity
{
    Task<List<T>> GetAll(CancellationToken ct = default);
    Task<ErrorOr<T>> GetBy(int id, CancellationToken ct = default);
    Task<ErrorOr<Created>> CreateAsync(T entity, CancellationToken ct = default);
    Task<ErrorOr<Updated>> UpdateAsync(T entity, CancellationToken ct = default);
    Task<ErrorOr<Deleted>> DeleteAsync(int id, CancellationToken ct = default);
}