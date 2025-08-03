using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Domain.Repositories;

public interface IBalanceRepository<TFilter> : IRepository<Balance> 
    where TFilter : class 
{
    Task<List<Balance>> FilterAsync(TFilter filter);
}
