using Microsoft.EntityFrameworkCore;
using WarehouseManagement.DataBase;
using WarehouseManagement.DataBase.Seeds;

namespace WarehouseManagement.Server.Extensions;

public static class ApplicationBuilderExtension
{
    public static async Task<IApplicationBuilder> MigrateDataBase(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var sp = scope.ServiceProvider;
        using var db = sp.GetRequiredService<WarehouseDbContext>();
        db.Database.Migrate();

        await Seeder.Seed(db, sp);

        return app;
    }
}