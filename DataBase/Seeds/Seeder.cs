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

        if (!db.Set<Balance>().Any())
        {
            logger.LogInformation("Seeding balances...");

            foreach (var el in Balances.Get())
                db.Add(el);
        }

        if (!db.Set<ReceiptDocument>().Any())
        {
            logger.LogInformation("Seeding receipts document...");

            foreach (var el in ReceiptsDocument.Get())
                db.Add(el);
        }

        if (!db.Set<ReceiptResource>().Any())
        {
            logger.LogInformation("Seeding receipts resource...");

            foreach (var el in ReceiptsResource.Get())
                db.Add(el);
        }

        if (!db.Set<ShipmentDocument>().Any())
        {
            logger.LogInformation("Seeding shipments document...");

            foreach (var el in ShipmentsDocument.Get())
                db.Add(el);
        }

        if (!db.Set<ShipmentResource>().Any())
        {
            logger.LogInformation("Seeding shipments resource...");

            foreach (var el in ShipmentsResource.Get())
                db.Add(el);
        }

        await db.SaveChangesAsync();
    }
}
