using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Seeds;

public sealed class Seeder
{
    public static async Task Seed(
        WarehouseDbContext db,
        IServiceProvider sp
    )
    {
        var logger = sp.GetRequiredService<ILogger<Seeder>>();

        if (!db.Set<Client>().Any())
        {
            logger.LogInformation("Seeding clients...");

            foreach (var el in Clients.Get())
                db.Add(el);
        }

        if (!db.Set<Resource>().Any())
        {
            logger.LogInformation("Seeding resources...");

            foreach (var el in Resources.Get())
                db.Add(el);
        }

        if (!db.Set<UnitOfMeasure>().Any())
        {
            logger.LogInformation("Seeding units of measure...");

            foreach (var el in Units.Get())
                db.Add(el);
        }

        await db.SaveChangesAsync();

        if (!db.Set<Balance>().Any())
        {
            logger.LogInformation("Seeding balances...");

            foreach (var el in Balances.Get())
                db.Add(el);
        }

        if (!db.Set<ReceiptDocument>().Any())
        {
            logger.LogInformation("Seeding receipt documents...");

            foreach (var el in ReceiptDocuments.Get())
                db.Add(el);
        }

        await db.SaveChangesAsync();

        if (!db.Set<ReceiptResource>().Any())
        {
            logger.LogInformation("Seeding receipt resources...");

            foreach (var el in ReceiptResources.Get())
                db.Add(el);
        }

        if (!db.Set<ShipmentDocument>().Any())
        {
            logger.LogInformation("Seeding shipment documents...");

            foreach (var el in ShipmentDocuments.Get())
                db.Add(el);
        }

        await db.SaveChangesAsync();

        if (!db.Set<ShipmentResource>().Any())
        {
            logger.LogInformation("Seeding shipment resources...");

            foreach (var el in ShipmentResources.Get()) 
                db.Add(el);
        }

        await db.SaveChangesAsync();
    }
}
