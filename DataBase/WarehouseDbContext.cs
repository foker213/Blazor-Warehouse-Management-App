using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;
using WarehouseManagement.Application;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase;

public class WarehouseDbContext : DbContext, IUnitOfWork
{
    public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options)
        : base(options)
    {
    }

    public DbSet<Balance> Balances => Set<Balance>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<ReceiptDocument> ReceiptDocuments => Set<ReceiptDocument>();
    public DbSet<ReceiptResource> ReceiptResources => Set<ReceiptResource>();
    public DbSet<Resource> Resources => Set<Resource>();
    public DbSet<ShipmentDocument> ShipmentDocuments => Set<ShipmentDocument>();
    public DbSet<ShipmentResource> ShipmentResources => Set<ShipmentResource>();
    public DbSet<UnitOfMeasure> UnitsOfMeasure => Set<UnitOfMeasure>();

    public DbTransaction? DbTransaction => Database.CurrentTransaction?.GetDbTransaction();

    public async Task<DbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await Database.BeginTransactionAsync(cancellationToken);
        return transaction.GetDbTransaction();
    }

    public async Task<Error?> CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            await Database.CommitTransactionAsync(cancellationToken);

            return default;
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            return Error.Failure("OperationFailed", "Не удалось выполнить операцию");
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default) =>
        await Database.RollbackTransactionAsync(cancellationToken);

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => base.SaveChangesAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
