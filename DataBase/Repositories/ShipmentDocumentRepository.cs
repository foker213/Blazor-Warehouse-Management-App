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
    public Task<ErrorOr<Updated>> ChangeStatusAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<List<ShipmentDocument>> FilterAsync(FilterDto filter, CancellationToken ct = default)
    {
        IQueryable<ShipmentDocument> query = GetQuery();

        if (!string.IsNullOrEmpty(filter.Number))
        {
            query = query.Where(x => x.Number.ToLower() == filter.Number.ToLower());
        }

        if (!string.IsNullOrEmpty(filter.Client))
        {
            query = query.Where(x => x.Client.Name.ToLower() == filter.Client.ToLower());
        }

        if (!string.IsNullOrEmpty(filter.Resource))
        {
            query = query.Where(x => x.ShipmentResources
                .Any(r => r.Resource != null &&
                         r.Resource.Name.ToLower() == filter.Resource.ToLower()));
        }

        if (!string.IsNullOrEmpty(filter.UnitOfMeasure))
        {
            query = query.Where(x => x.ShipmentResources
                .Any(r => r.UnitOfMeasure != null &&
                         r.UnitOfMeasure.Name.ToLower() == filter.UnitOfMeasure.ToLower()));
        }

        return await query.ToListAsync(ct);
    }

    public async Task<ShipmentDocument?> GetByNumber(string number, CancellationToken ct = default)
    {
        IQueryable<ShipmentDocument> query = GetQuery();
        return await query.Where(x => x.Number.ToLower() == number.ToLower()).FirstOrDefaultAsync(ct);
    }
}