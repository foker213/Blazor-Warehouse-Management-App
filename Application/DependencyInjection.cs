using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using WarehouseManagement.Application.IServices;
using WarehouseManagement.Application.Services;

namespace WarehouseManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Configuring Mapster
        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton(typeAdapterConfig);
        services.AddScoped<IMapper, ServiceMapper>();

        services.AddScoped<IBalanceService, BalanceService>();
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IReceiptDocumentService, ReceiptDocumentService>();
        services.AddScoped<IResourceService, ResourceService>();
        services.AddScoped<IShipmentDocumentService, ShipmentDocumentService>();
        services.AddScoped<IUnitOfMeasureService, UnitOfMeasureService>();
        services.AddScoped<IUnitOfMeasureService, UnitOfMeasureService>();

        return services;
    }
}
