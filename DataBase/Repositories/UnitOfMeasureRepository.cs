using ErrorOr;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Application.IRepositories;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Repositories;

internal sealed class UnitOfMeasureRepository(WarehouseDbContext db) : Repository<UnitOfMeasure>(db), IUnitOfMeasureRepository
{
    public Task<ErrorOr<Updated>> ChangeStateAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
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