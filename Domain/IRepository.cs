namespace WarehouseManagement.Domain;

public interface IRepository<T> where T : class, IEntity
{
    Task<List<T>> GetAll();
    Task<T?> GetBy(int id);
    Task CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}