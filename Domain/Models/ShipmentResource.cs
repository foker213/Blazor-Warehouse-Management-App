namespace WarehouseManagement.Domain.Models;

public class ShipmentResource : Entity
{
    public Resource Resource { get; private set; } = default!;
    public int ResourceId { get; set; }

    /// <summary>
    /// Прикрепленная единица измерения
    /// </summary>
    public UnitOfMeasure UnitOfMeasure { get; private set; } = default!;
    public int UnitOfMeasureId { get; private set; }

    /// <summary>
    /// Количество
    /// </summary>
    public int Quantity { get; private set; }

    public int ShipmentDocumentId { get; private set; }
    public ShipmentDocument ShipmentDocument { get; private set; } = default!;
}
