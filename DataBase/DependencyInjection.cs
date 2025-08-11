using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WarehouseManagement.Application;
using WarehouseManagement.Application.IRepositories;
using WarehouseManagement.DataBase.Repositories;

namespace WarehouseManagement.DataBase;

public static class DependencyInjection
{
    public static IServiceCollection AddDataBase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WarehouseDbContext>(options =>
            options.UseNpgsql(configuration["DbContext:ConnectionString"]));

        services.AddScoped<IBalanceRepository, BalanceRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IReceiptDocumentRepository, ReceiptDocumentRepository>();
        services.AddScoped<IResourceRepository, ResourceRepository>();
        services.AddScoped<IShipmentDocumentRepository, ShipmentDocumentRepository>();
        services.AddScoped<IUnitOfMeasureRepository, UnitOfMeasureRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<WarehouseDbContext>());

        return services;
    }
}