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

        config.NewConfig<ReceiptDocumentDto, ReceiptDocument>();
        config.NewConfig<ReceiptDocument, ReceiptDocumentDto>();
        config.NewConfig<ReceiptResourceDto, ReceiptResource>();
        config.NewConfig<ReceiptResource, ReceiptResourceDto>();
        config.NewConfig<List<ReceiptResource>, List<ReceiptResourceDto>>();
        config.NewConfig<List<ReceiptResourceDto>, List<ReceiptResource>>();

        config.NewConfig<ShipmentDocumentDto, ShipmentDocument>();
        config.NewConfig<ShipmentDocument, ShipmentDocumentDto>();
        config.NewConfig<ShipmentResourceDto, ShipmentResource>();
        config.NewConfig<ShipmentResource, ShipmentResourceDto>();
        config.NewConfig<List<ShipmentResource>, List<ShipmentResourceDto>>();
        config.NewConfig<List<ShipmentResourceDto>, List<ShipmentResource>>();

        config.NewConfig<ResourceDto, Resource>();
        config.NewConfig<Resource, ResourceDto>();

        config.NewConfig<UnitOfMeasureDto, UnitOfMeasure>();
        config.NewConfig<UnitOfMeasure, UnitOfMeasureDto>();
    }
}
