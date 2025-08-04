using ErrorOr;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Application.IRepositories;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Repositories;

internal sealed class ResourceRepository(WarehouseDbContext db) : Repository<Resource>(db), IResourceRepository
{
    public Task<ErrorOr<Updated>> ChangeStateAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public override async Task<ErrorOr<Deleted>> DeleteAsync(Resource resource, CancellationToken ct = default)
    {
        DbSet.Remove(resource);
        try
        {
            await db.SaveChangesAsync(ct);
            return new Deleted();
        }
        catch
        {
            return Error.Failure("DeleteFailed", "Ошибка при удалении ресурса");
        }
    }

    public async Task<Resource?> GetByName(string name, CancellationToken ct = default)
    {
        IQueryable<Resource> query = GetQuery();
        return await query.Where(x => x.Name.ToLower() == name.ToLower()).FirstOrDefaultAsync(ct);
    }
}