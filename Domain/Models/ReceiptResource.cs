using ErrorOr;

namespace WarehouseManagement.Domain.Models;

public class ReceiptResource : Entity
{
    /// <summary>
    /// Прикрепленный ресурс
    /// </summary>
    public Resource Resource { get; private set; } = default!;
    public int ResourceId { get; private set; }

    /// <summary>
    /// Прикрепленная единица измерения
    /// </summary>
    public UnitOfMeasure UnitOfMeasure { get; private set; } = default!;
    public int UnitOfMeasureId { get; private set; }

    /// <summary>
    /// Количество
    /// </summary>
    public int Quantity { get; private set; }


    /// <summary>
    /// Количество
    /// </summary>
    public ReceiptDocument ReceiptDocument { get; private set; } = default!;
    public int ReceiptDocumentId { get; private set; }

    private int _oldQuantity;

    protected ReceiptResource() { }

    public static ErrorOr<ReceiptResource> Create(
        int resourceId,
        int unitId,
        int quantity,
        ReceiptDocument document)
    {
        if (quantity <= 0)
            return Error.Validation("InvalidQuantity", "Количество должно быть положительным");

        if (document == null)
            return Error.Validation("DocumentRequired", "Документ обязателен");

        return new ReceiptResource
        {
            ResourceId = resourceId,
            UnitOfMeasureId = unitId,
            Quantity = quantity,
            ReceiptDocumentId = document.Id
        };
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
