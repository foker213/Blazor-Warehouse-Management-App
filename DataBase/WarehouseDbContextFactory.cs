using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WarehouseManagement.DataBase;

public class WarehouseDbContextFactory : IDesignTimeDbContextFactory<WarehouseDbContext>
{
    public WarehouseDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<WarehouseDbContext>();
        optionsBuilder
            .UseNpgsql("Server=localhost;Port=5432;Database=Warehouse;User Id=guest;Password=guest;Pooling=true;");

        return new WarehouseDbContext(optionsBuilder.Options);
    }
}
