using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Application.IRepositories;
using WarehouseManagement.Contracts;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Repositories;

internal sealed class ReceiptDocumentRepository(WarehouseDbContext db) :
    Repository<ReceiptDocument>(db),
    IReceiptDocumentRepository
{
    protected override IQueryable<ReceiptDocument> GetQuery()
    {
#nullable disable
        return DbSet.AsNoTracking()
            .Include(x => x.ReceiptResources)
                .ThenInclude(x => x.Resource)
            .Include(x => x.ReceiptResources)
                .ThenInclude(x => x.UnitOfMeasure);
#nullable restore
    }

    public async Task<List<ReceiptDocument>> FilterAsync(FilterDto filter, CancellationToken ct = default)
    {
        IQueryable<ReceiptDocument> query = GetQuery();

        if (!string.IsNullOrEmpty(filter.Number))
        {
            query = query.Where(x => x.Number.ToLower() == filter.Number.ToLower());
        }

        query = query.Where(x => x.Date >= filter.DateStart!.Value);

        DateOnly endDate = filter.DateEnd!.Value.AddDays(1);
        query = query.Where(x => x.Date < endDate);

        query = query.Where(x => x.ReceiptResources != null && x.ReceiptResources.Any());

        if (!string.IsNullOrEmpty(filter.Resource))
        {
            query = query.Where(x => x.ReceiptResources!
                .Any(r => r.Resource != null &&
                         r.Resource.Name.ToLower() == filter.Resource.ToLower()));
        }

        if (!string.IsNullOrEmpty(filter.UnitOfMeasure))
        {
            query = query.Where(x => x.ReceiptResources!
                .Any(r => r.UnitOfMeasure != null &&
                         r.UnitOfMeasure.Name.ToLower() == filter.UnitOfMeasure.ToLower()));
        }

        return await query.ToListAsync(ct);
    }

    public async Task<ReceiptDocument?> GetByNumber(string number, CancellationToken ct = default)
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