using ErrorOr;

namespace WarehouseManagement.Domain.Models;

public class ReceiptResource : Entity
{
    /// <summary>
    /// Прикрепленный ресурс
    /// </summary>
    public Resource? Resource { get; private set; } = default!;

    /// <summary>
    /// Id прикрепленного ресурса
    /// </summary>
    public int ResourceId { get; private set; }

    /// <summary>
    /// Прикрепленная единица измерения
    /// </summary>
    public UnitOfMeasure? UnitOfMeasure { get; private set; } = default!;

    /// <summary>
    /// Id прикрепленной единицы измерения
    /// </summary>
    public int UnitOfMeasureId { get; private set; }

    /// <summary>
    /// Количество
    /// </summary>
    public int Quantity { get; private set; }
    public ReceiptDocument ReceiptDocument { get; private set; } = default!;
    public int ReceiptDocumentId { get; private set; }

    private int _oldQuantity = 0;

    protected ReceiptResource() { }

    public static ErrorOr<ReceiptResource> Create(
        int resourceId,
        int unitId,
        int quantity,
        int documentId)
    {
        if (quantity < 0)
            return Error.Validation("InvalidQuantity", "Количество должно быть положительным");

        return new ReceiptResource
        {
            ResourceId = resourceId,
            UnitOfMeasureId = unitId,
            Quantity = quantity,
            ReceiptDocumentId = documentId
        };
    }

    public void Update(int resourceId, int unitOfMeasureId, int quantity)
    {
        ResourceId = resourceId;
        UnitOfMeasureId = unitOfMeasureId;
        Quantity = quantity;
    }

    public void ChangeQuantity(int quantity)
    {
        _oldQuantity = Quantity;
        Quantity = quantity;
    }

    public int RecalculateDifference()
    {
        return _oldQuantity - Quantity;
    }
}
