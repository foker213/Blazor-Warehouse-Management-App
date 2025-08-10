using WarehouseManagement.Domain.Enums;

namespace WarehouseManagement.Domain.Models;

public class ShipmentDocument : Entity
{
    /// <summary>
    /// Номер поступления
    /// </summary>
    public string Number { get; private set; } = default!;

    /// <summary>
    /// Прикрепленный клиент
    /// </summary>
    public Client Client { get; private set; } = default!;
    public int ClientId { get; private set; }

    /// <summary>
    /// Дата поступления
    /// </summary>
    public DateOnly Date { get; private set; }

    /// <summary>
    /// Статус подписания
    /// </summary>
    public Status Status { get; private set; }

    /// <summary>
    /// Прикрепленные ресурсы отгрузки
    /// </summary>
    public List<ShipmentResource> ShipmentResources { get; private set; } = default!;

    protected ShipmentDocument() { }
}
