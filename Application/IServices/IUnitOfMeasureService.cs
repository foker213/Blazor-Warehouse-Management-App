using ErrorOr;
using WarehouseManagement.Contracts.UnitOfMeasure;

namespace WarehouseManagement.Application.IServices;

public interface IUnitOfMeasureService
{
    Task<List<UnitOfMeasureDto>> GetAll(CancellationToken ct = default);
    Task<ErrorOr<UnitOfMeasureDto>> GetBy(int id, CancellationToken ct = default);
    Task<ErrorOr<Created>> CreateAsync(UnitOfMeasureDto entity, CancellationToken ct = default);
    Task<ErrorOr<Updated>> UpdateAsync(UnitOfMeasureDto entity, CancellationToken ct = default);
    Task<ErrorOr<Deleted>> DeleteAsync(int id, CancellationToken ct = default);
    Task<ErrorOr<Updated>> ChangeStateAsync(int id, CancellationToken ct = default);
}
