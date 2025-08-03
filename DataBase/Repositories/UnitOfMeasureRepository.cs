using WarehouseManagement.Domain.Models;
using WarehouseManagement.Domain.Repositories;

namespace WarehouseManagement.DataBase.Repositories;

internal sealed class UnitOfMeasureRepository(WarehouseDbContext db) : Repository<UnitOfMeasure>(db), IUnitOfMeasureRepository
{
    public Task ChangeStatusAsync(int id)
    {
        throw new NotImplementedException();
    }
}