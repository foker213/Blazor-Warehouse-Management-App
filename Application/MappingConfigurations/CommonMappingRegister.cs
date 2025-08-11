using Mapster;
using WarehouseManagement.Contracts.Balance;
using WarehouseManagement.Contracts.Client;
using WarehouseManagement.Contracts.Receipt;
using WarehouseManagement.Contracts.Resource;
using WarehouseManagement.Contracts.Shipment;
using WarehouseManagement.Contracts.UnitOfMeasure;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Application.MappingConfigurations;

public class CommonMappingRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<BalanceDto, Balance>();
        config.NewConfig<Balance, BalanceDto>();

        config.NewConfig<ClientDto, Client>();
        config.NewConfig<Client, ClientDto>();
        config.NewConfig<ClientCreateDto, Resource>();
        config.NewConfig<ClientUpdateDto, Resource>();

        config.NewConfig<ReceiptDocument, ReceiptDocumentDto>()
            .Map(dest => dest.Date, src => src.Date.ToDateTime(TimeOnly.MinValue))
            .Map(dest => dest.ReceiptResources, src => src.ReceiptResources.Adapt<List<ReceiptResourceDto>>());

        config.NewConfig<ReceiptResource, ReceiptResourceDto>()
            .Map(dest => dest.Resource, src => src.Resource.Adapt<ResourceDto>())
            .Map(dest => dest.UnitOfMeasure, src => src.UnitOfMeasure.Adapt<UnitDto>());

        config.NewConfig<ReceiptDocument, ReceiptDocumentUpdateDto>()
            .Map(dest => dest.Date, src => src.Date.ToDateTime(TimeOnly.MinValue))
            .Map(dest => dest.ReceiptResources, src => src.ReceiptResources.Adapt<List<ReceiptResourceUpdateDto>>());

        config.NewConfig<ReceiptResource, ReceiptResourceUpdateDto>();

        config.NewConfig<ShipmentDocument, ShipmentDocumentDto>()
            .Map(dest => dest.Date, src => src.Date.ToDateTime(TimeOnly.MinValue))
            .Map(dest => dest.ShipmentResources, src => src.ShipmentResources.Adapt<List<ShipmentResourceDto>>());

        config.NewConfig<ShipmentResource, ShipmentResourceDto>()
            .Map(dest => dest.Resource, src => src.Resource.Adapt<ResourceDto>())
            .Map(dest => dest.UnitOfMeasure, src => src.UnitOfMeasure.Adapt<UnitDto>());

        config.NewConfig<ResourceDto, Resource>();
        config.NewConfig<Resource, ResourceDto>();
        config.NewConfig<ResourceCreateDto, Resource>();
        config.NewConfig<ResourceUpdateDto, Resource>();

        config.NewConfig<UnitDto, UnitOfMeasure>();
        config.NewConfig<UnitOfMeasure, UnitDto>();
        config.NewConfig<UnitCreateDto, Resource>();
        config.NewConfig<UnitUpdateDto, Resource>();
    }
}
