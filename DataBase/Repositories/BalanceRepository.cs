using WarehouseManagement.Contracts;
using WarehouseManagement.Domain.Models;
using WarehouseManagement.Domain.Repositories;

namespace WarehouseManagement.DataBase.Repositories;

internal sealed class BalanceRepository(WarehouseDbContext db) :
    Repository<Balance>(db),
    IBalanceRepository<FilterDto>
{
    public Task<List<Balance>> FilterAsync(FilterDto filter)
    {
        throw new NotImplementedException();
    }
}
