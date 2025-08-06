using ErrorOr;
using WarehouseManagement.Contracts.UnitOfMeasure;

namespace WarehouseManagement.Application.IServices;

public interface IUnitOfMeasureService
{
    Task<List<UnitDto>> GetAll(CancellationToken ct = default);
    Task<ErrorOr<UnitDto>> GetBy(int id, CancellationToken ct = default);
    Task<ErrorOr<Created>> CreateAsync(UnitCreateDto entity, CancellationToken ct = default);
    Task<ErrorOr<Updated>> UpdateAsync(UnitUpdateDto entity, CancellationToken ct = default);
    Task<ErrorOr<Deleted>> DeleteAsync(int id, CancellationToken ct = default);
    Task<ErrorOr<Updated>> ChangeStateAsync(int id, CancellationToken ct = default);
}
