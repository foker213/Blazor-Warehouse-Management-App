using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Domain;

namespace WarehouseManagement.DataBase;

public abstract class Repository<T> : IRepository<T> where T : class, IEntity
{
    protected readonly WarehouseDbContext _db;

    public Repository(WarehouseDbContext db)
    {
        _db = db;
    }

    protected DbSet<T> DbSet => _db.Set<T>();

    private IQueryable<T> GetQuery()
    {
        return DbSet.AsQueryable();
    }
    public async Task<List<T>> GetAll()
    {
        IQueryable<T> query = GetQuery();
        return await query.ToListAsync();
    }

    public virtual async Task<T?> GetBy(int id)
    {
        IQueryable<T> query = GetQuery();
        return await query.Where(x => x.Id == id).FirstOrDefaultAsync();
    }
    public virtual async Task CreateAsync(T entity)
    {
        DbSet.Add(entity);
        await _db.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(T entity)
    {
        DbSet.Update(entity);
        await _db.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(int id)
    {
        T entity = await GetBy(id) ?? throw new ArgumentNullException(nameof(id));
        DbSet.Remove(entity);
        await _db.SaveChangesAsync();
    }
}