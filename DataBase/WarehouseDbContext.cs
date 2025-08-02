using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase;

public class WarehouseDbContext : DbContext
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
