namespace WarehouseManagement.Domain.Models;

public class ShipmentResource : Entity
{
    public Resource Resource { get; set; } = default!;
    public int ResourceId { get; set; }

    /// <summary>
    /// Прикрепленная единица измерения
    /// </summary>
    public UnitOfMeasure UnitOfMeasure { get; set; } = default!;
    public int UnitOfMeasureId { get; set; }

    /// <summary>
    /// Количество
    /// </summary>
    public int Quantity { get; set; }

    public int ShipmentDocumentId { get; set; }
    public ShipmentDocument ShipmentDocument { get; set; } = default!;
}
