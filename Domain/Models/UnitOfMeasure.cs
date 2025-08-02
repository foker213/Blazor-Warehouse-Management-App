using WarehouseManagement.Domain.Enums;

namespace WarehouseManagement.Domain.Models;

public class UnitOfMeasure : Entity
{
    /// <summary>
    /// Наименование
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Состояние
    /// </summary>
    public State State { get; set; }

    public Balance? Balance { get; set; }
    public ReceiptResource? ReceiptResource { get; set; }
    public ShipmentResource? ShipmentResource{ get; set; }
}
