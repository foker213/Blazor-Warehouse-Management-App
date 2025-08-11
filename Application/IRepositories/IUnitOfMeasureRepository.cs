using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Application.IRepositories;

public interface IUnitOfMeasureRepository : IRepository<UnitOfMeasure>
{
    Task ChangeStateAsync(UnitOfMeasure unit, CancellationToken ct = default);
    Task<UnitOfMeasure?> GetByName(string name, CancellationToken ct = default);
}
