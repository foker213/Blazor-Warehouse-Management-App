using WarehouseManagement.Domain.Enums;

namespace WarehouseManagement.Domain.Models;

public class Client : Entity
{
    /// <summary>
    /// Наименование
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Адрес
    /// </summary>
    public required string Adress { get; set; }

    /// <summary>
    /// Состояние
    /// </summary>
    public State State { get; set; }

    public List<ShipmentDocument>? ShipmentDocuments { get; set; }
}
