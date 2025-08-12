using ErrorOr;
using WarehouseManagement.Contracts.Resource;

namespace WarehouseManagement.Application.IServices;

public interface IResourceService
{
    Task<List<ResourceDto>> GetAll(CancellationToken ct);
    Task<ErrorOr<ResourceDto>> GetBy(int id, CancellationToken ct);
    Task<ErrorOr<Created>> CreateAsync(ResourceCreateDto entity, CancellationToken ct);
    Task<ErrorOr<Updated>> UpdateAsync(ResourceUpdateDto entity, CancellationToken ct);
    Task<ErrorOr<Deleted>> DeleteAsync(int id, CancellationToken ct);
    Task<ErrorOr<Updated>> ChangeStateAsync(int id, CancellationToken ct);
}
