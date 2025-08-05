using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Application.IRepositories;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Repositories;

internal sealed class ResourceDocumentRepository : IReceiptResourceRepository
{
    private readonly WarehouseDbContext _db;

    public ResourceDocumentRepository(WarehouseDbContext db)
    {
        _db = db;
    }

    private DbSet<ReceiptResource> DbSet => _db.Set<ReceiptResource>();

    private IQueryable<ReceiptResource> GetQuery()
    {
        return DbSet.AsQueryable();
    }

    public async Task<int> GetTotalQuantity(int resourceId, int unitId, CancellationToken ct = default)
    {
        IQueryable<ReceiptResource> query = GetQuery();
        return await query.Where(x => x.ResourceId == resourceId && x.UnitOfMeasureId == unitId).SumAsync(x => x.Quantity, ct);
    }
}
