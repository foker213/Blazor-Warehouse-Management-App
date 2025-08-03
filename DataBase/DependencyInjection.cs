using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WarehouseManagement.Contracts;
using WarehouseManagement.DataBase.Repositories;
using WarehouseManagement.Domain.Repositories;

namespace WarehouseManagement.DataBase;

public static class DependencyInjection
{
    public static IServiceCollection AddDataBase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WarehouseDbContext>(options =>
            options.UseNpgsql(configuration["DbContext:ConnectionString"]));

        services.AddScoped<IBalanceRepository<FilterDto>, BalanceRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IReceiptDocumentRepository<FilterDto>, ReceiptDocumentRepository>();
        services.AddScoped<IResourceRepository, ResourceRepository>();
        services.AddScoped<IShipmentDocumentRepository<FilterDto>, ShipmentDocumentRepository>();
        services.AddScoped<IUnitOfMeasureRepository, UnitOfMeasureRepository>();


        return services;
    }
}