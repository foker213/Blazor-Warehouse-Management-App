using WarehouseManagement.Contracts.Resource;
using WarehouseManagement.Contracts.UnitOfMeasure;

namespace WarehouseManagement.Contracts.Shipment;

public class ShipmentResourceDto
{
    public int Id { get; set; }
    public required ResourceDto Resource { get; set; }
    public required UnitOfMeasureDto Unit { get; set; }
    public int Quantity { get; set; }
}
