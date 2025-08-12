using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.Contracts.Shipment;

public class ShipmentResourceCreateDto
{
    public int ResourceId { get; set; }

    public int UnitOfMeasureId { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Количество должно быть положительным")]
    public int Quantity { get; set; }
}
