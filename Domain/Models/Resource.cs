using WarehouseManagement.Domain.Enums;

namespace WarehouseManagement.Domain.Models;

public class Resource : Entity
{
    /// <summary>
    /// Наименование
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Состояние
    /// </summary>
    public State State { get; set; }

    public ICollection<Balance>? Balances { get; set; }
    public ICollection<ReceiptResource>? ReceiptResources { get; set; }
    public ICollection<ShipmentResource>? ShipmentResources { get; set; }
}
