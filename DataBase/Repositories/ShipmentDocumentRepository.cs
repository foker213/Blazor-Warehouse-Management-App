using ErrorOr;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Application.IRepositories;
using WarehouseManagement.Contracts;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Repositories;

internal sealed class ShipmentDocumentRepository(WarehouseDbContext db) :
    Repository<ShipmentDocument>(db),
    IShipmentDocumentRepository
{
    protected override IQueryable<ShipmentDocument> GetQuery()
    {
        return DbSet.AsNoTracking()
            .Include(x => x.ShipmentResources)
                .ThenInclude(x => x.Resource)
            .Include(x => x.ShipmentResources)
                .ThenInclude(x => x.UnitOfMeasure)
            .Include(x => x.Client);
    }

    public Task<ErrorOr<Updated>> ChangeStatusAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<List<ShipmentDocument>> FilterAsync(FilterDto filter, CancellationToken ct = default)
    {
        IQueryable<ShipmentDocument> query = GetQuery();

        if (filter.Resources is not null && filter.Resources.Count > 0)
            query = query.Where(x => x.ShipmentResources != null &&
                                    x.ShipmentResources.Any(rr => filter.Resources.Contains(rr.Resource.Name)));

        if (filter.UnitsOfMeasure is not null && filter.UnitsOfMeasure.Count > 0)
            query = query.Where(x => x.ShipmentResources != null &&
                                    x.ShipmentResources.Any(rr => filter.UnitsOfMeasure.Contains(rr.UnitOfMeasure.Name)));

        if (filter.Numbers is not null && filter.Numbers.Count > 0)
            query = query.Where(x => filter.Numbers.Any(s => s == x.Number));

        if (filter.Clients is not null && filter.Clients.Count > 0)
            query = query.Where(x => filter.Clients.Any(s => s == x.Client.Name));

        query = query.Where(x => x.Date >= filter.DateStart!.Value);

        DateOnly endDate = filter.DateEnd!.Value.AddDays(1);
        query = query.Where(x => x.Date < endDate);

        return await query.ToListAsync(ct);
    }

    public async Task<ShipmentDocument?> GetByNumber(string number, CancellationToken ct = default)
    {
        IQueryable<ShipmentDocument> query = GetQuery();
        return await query.Where(x => x.Number.ToLower() == number.ToLower()).FirstOrDefaultAsync(ct);
    }
}