using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Application.IRepositories;
using WarehouseManagement.Contracts;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Repositories;

internal sealed class ReceiptDocumentRepository(WarehouseDbContext db) :
    Repository<ReceiptDocument>(db),
    IReceiptDocumentRepository
{
    protected override IQueryable<ReceiptDocument> GetQuery(bool isTracked = false)
    {
        if(isTracked)
            return DbSet.AsQueryable().Include(x => x.ReceiptResources);
        else
#nullable disable
            return DbSet.AsNoTracking()
                .Include(x => x.ReceiptResources)
                    .ThenInclude(x => x.Resource)
                .Include(x => x.ReceiptResources)
                    .ThenInclude(x => x.UnitOfMeasure);
#nullable restore
    }

    public override async Task<ReceiptDocument?> GetByAsync(int id, CancellationToken ct = default)
    {
        IQueryable<ReceiptDocument> query = GetQuery(true);
        return await query.Where(x => x.Id == id).FirstOrDefaultAsync(ct);
    }

    public async Task<List<ReceiptDocument>> FilterAsync(FilterDto filter, CancellationToken ct = default)
    {
        IQueryable<ReceiptDocument> query = GetQuery();

        if (filter.Resources is not null && filter.Resources.Count > 0)
            query = query.Where(x => x.ReceiptResources != null &&
                            x.ReceiptResources.Any(rr =>
                                rr != null &&
                                rr.Resource != null &&
                                filter.Resources.Contains(rr.Resource.Name)));

        if (filter.UnitsOfMeasure is not null && filter.UnitsOfMeasure.Count > 0)
            query = query.Where(x => x.ReceiptResources != null &&
                            x.ReceiptResources.Any(rr =>
                                rr != null &&
                                rr.UnitOfMeasure != null &&
                                filter.UnitsOfMeasure.Contains(rr.UnitOfMeasure.Name)));

        if (filter.Numbers is not null && filter.Numbers.Count > 0)
            query = query.Where(x => filter.Numbers.Any(s => s == x.Number));

        if (filter.DateStart is not null)
            query = query.Where(x => x.Date >= filter.DateStart!.Value);

        if (filter.DateEnd is not null)
        {
            DateOnly endDate = filter.DateEnd!.Value.AddDays(1);
            query = query.Where(x => x.Date < endDate);
        }

        return await query.ToListAsync(ct);
    }

    public async Task<ReceiptDocument?> GetByNumberAsync(string number, CancellationToken ct = default)
    {
        IQueryable<ReceiptDocument> query = GetQuery();
        return await query.Where(x => x.Number.ToLower() == number.ToLower()).FirstOrDefaultAsync(ct);
    }

    public void Add(ReceiptDocument document)
        => DbSet.Add(document);

    public void Update(ReceiptDocument document)
        => DbSet.Update(document);

    public void Remove(ReceiptDocument document)
        => DbSet.Remove(document);
}