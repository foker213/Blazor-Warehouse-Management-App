using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Application.IRepositories;
using WarehouseManagement.Contracts;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Repositories;

internal sealed class ReceiptDocumentRepository(WarehouseDbContext db) :
    Repository<ReceiptDocument>(db),
    IReceiptDocumentRepository
{
    public Task<List<ReceiptDocument>> FilterAsync(FilterDto filter, CancellationToken ct = default)
    {
        throw new NotImplementedException();
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