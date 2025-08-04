using ErrorOr;
using WarehouseManagement.Contracts.Resource;

namespace WarehouseManagement.Application.IServices;

public interface IResourceService
{
    Task<List<ResourceDto>> GetAll(CancellationToken ct = default);
    Task<ErrorOr<ResourceDto>> GetBy(int id, CancellationToken ct = default);
    Task<ErrorOr<Created>> CreateAsync(ResourceDto entity, CancellationToken ct = default);
    Task<ErrorOr<Updated>> UpdateAsync(ResourceDto entity, CancellationToken ct = default);
    Task<ErrorOr<Deleted>> DeleteAsync(int id, CancellationToken ct = default);
    Task<ErrorOr<Updated>> ChangeStateAsync(int id, CancellationToken ct = default);
}
