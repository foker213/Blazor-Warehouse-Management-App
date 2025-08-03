using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Domain.Repositories;

public interface IUnitOfMeasureRepository : IRepository<UnitOfMeasure>
{
    Task ChangeStatusAsync(int id);
}
