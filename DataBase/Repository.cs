using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Application;
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

    protected virtual IQueryable<T> GetQuery()
    {
        return DbSet.AsQueryable();
    }
    public async Task<List<T>> GetAllAsync(CancellationToken ct = default)
    {
        IQueryable<T> query = GetQuery();
        return await query.ToListAsync(ct);
    }

    public virtual async Task<T?> GetByAsync(int id, CancellationToken ct = default)
    {
        IQueryable<T> query = GetQuery();
        return await query.Where(x => x.Id == id).FirstOrDefaultAsync(ct);
    }

    public virtual async Task CreateAsync(T entity, CancellationToken ct = default)
    {
        DbSet.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public virtual async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        DbSet.Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public virtual async Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        DbSet.Remove(entity);
        await _db.SaveChangesAsync(ct);
    }
}