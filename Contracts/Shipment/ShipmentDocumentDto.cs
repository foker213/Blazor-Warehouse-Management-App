using WarehouseManagement.Contracts.Client;
using WarehouseManagement.Contracts.Enums;

namespace WarehouseManagement.Contracts.Shipment;

public class ShipmentDocumentDto
{
    public int Id { get; set; }
    public required string Number { get; set; }
    public required ClientDto Client { get; set; }
    public DateTime Date { get; set; }
    public Status Status { get; set; }
    public required List<ShipmentResourceDto> ShipmentResources { get; set; }
}
