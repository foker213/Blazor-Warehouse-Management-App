using ErrorOr;

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
    public ShipmentDocument? ShipmentDocument { get; private set; } = default!;

    private int _oldQuantity = 0;

    public static ErrorOr<ShipmentResource> Create(
        int resourceId,
        int unitId,
        int quantity,
        int documentId)
    {
        if (quantity <= 0)
            return Error.Validation("InvalidQuantity", "Количество должно быть положительным");

        return new ShipmentResource
        {
            ResourceId = resourceId,
            UnitOfMeasureId = unitId,
            Quantity = quantity,
            ShipmentDocumentId = documentId
        };
    }

    public void ChangeQuantity(int quantity)
    {
        _oldQuantity = Quantity;
        Quantity = quantity;
    }

    public int RecalculateDifference()
    {
        return Quantity - _oldQuantity;
    }
}
