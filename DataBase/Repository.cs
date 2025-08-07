using ErrorOr;
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
        return DbSet.AsNoTracking();
    }
    public async Task<List<T>> GetAll(CancellationToken ct = default)
    {
        IQueryable<T> query = GetQuery();
        return await query.ToListAsync(ct);
    }

    public virtual async Task<T?> GetBy(int id, CancellationToken ct = default)
    {
        IQueryable<T> query = GetQuery();
        return await query.Where(x => x.Id == id).FirstOrDefaultAsync(ct);
    }
    public virtual async Task<ErrorOr<Created>> CreateAsync(T entity, CancellationToken ct = default)
    {
        DbSet.Add(entity);

        await _db.SaveChangesAsync(ct);
        return new Created();
    }

    public virtual async Task<ErrorOr<Updated>> UpdateAsync(T entity, CancellationToken ct = default)
    {
        DbSet.Update(entity);

        await _db.SaveChangesAsync(ct);
        return new Updated();
    }

    public virtual async Task<ErrorOr<Deleted>> DeleteAsync(T entity, CancellationToken ct = default)
    {
        DbSet.Remove(entity);

        await _db.SaveChangesAsync(ct);
        return new Deleted();
    }
}