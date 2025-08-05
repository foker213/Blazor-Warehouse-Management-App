using ErrorOr;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Application.IRepositories;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Repositories;

internal sealed class UnitOfMeasureRepository(WarehouseDbContext db) : Repository<UnitOfMeasure>(db), IUnitOfMeasureRepository
{
    public async Task<ErrorOr<Updated>> ChangeStateAsync(UnitOfMeasure unit, CancellationToken ct = default)
    {
        DbSet.Entry(unit).Property(x => x.State).IsModified = true;
        await db.SaveChangesAsync();

        return new Updated();
    }

    public override async Task<ErrorOr<Deleted>> DeleteAsync(UnitOfMeasure unit, CancellationToken ct = default)
    {
        DbSet.Remove(unit);

        try
        {
            await db.SaveChangesAsync(ct);
            return new Deleted();
        }
        catch
        {
            return Error.Failure("DeleteFailed", "Ошибка при удалении единицы измерения");
        }
    }

    public async Task<UnitOfMeasure?> GetByName(string name, CancellationToken ct = default)
    {
        IQueryable<UnitOfMeasure> query = GetQuery();
        return await query.Where(x => x.Name.ToLower() == name.ToLower()).FirstOrDefaultAsync(ct);
    }
}