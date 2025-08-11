using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.Contracts.Shipment;

public class ShipmentResourceUpdateDto
{
    public int Id { get; set; } = 0;
    public int ResourceId { get; set; }
    public int UnitOfMeasureId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть положительным")]
    public int Quantity { get; set; }
}