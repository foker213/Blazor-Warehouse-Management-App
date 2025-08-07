namespace WarehouseManagement.Domain.Models;

public class ReceiptDocument : Entity
{
    /// <summary>
    /// Номер поступления
    /// </summary>
    public required string Number { get; set; }

    /// <summary>
    /// Дата поступления
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Прикрепленные ресурсы поступления
    /// </summary>
    public List<ReceiptResource>? ReceiptResources { get; set; }
}
