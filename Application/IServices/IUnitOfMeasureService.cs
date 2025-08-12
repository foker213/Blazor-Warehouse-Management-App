using ErrorOr;
using WarehouseManagement.Contracts.UnitOfMeasure;

namespace WarehouseManagement.Application.IServices;

public interface IUnitOfMeasureService
{
    Task<List<UnitDto>> GetAll(CancellationToken ct);
    Task<ErrorOr<UnitDto>> GetBy(int id, CancellationToken ct);
    Task<ErrorOr<Created>> CreateAsync(UnitCreateDto entity, CancellationToken ct);
    Task<ErrorOr<Updated>> UpdateAsync(UnitUpdateDto entity, CancellationToken ct);
    Task<ErrorOr<Deleted>> DeleteAsync(int id, CancellationToken ct);
    Task<ErrorOr<Updated>> ChangeStateAsync(int id, CancellationToken ct);
}
