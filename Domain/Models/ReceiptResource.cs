namespace WarehouseManagement.Domain.Models;

public class ReceiptResource : Entity
{
    /// <summary>
    /// Прикрепленный ресурс
    /// </summary>
    public Resource Resource { get; set; } = default!;
    public int ResourceId { get; set; }

    /// <summary>
    /// Прикрепленная единица измерения
    /// </summary>
    public UnitOfMeasure UnitOfMeasure { get; set; } = default!;
    public int UnitId { get; set; }

    /// <summary>
    /// Количество
    /// </summary>
    public int Quantity { get; set; }

    public int ReceiptDocumentId { get; set; }
    public ReceiptDocument ReceiptDocument { get; set; } = default!;
}
