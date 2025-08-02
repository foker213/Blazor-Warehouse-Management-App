using WarehouseManagement.Domain.Enums;

namespace WarehouseManagement.Domain.Models;

public class ShipmentDocument : Entity
{
    /// <summary>
    /// Номер поступления
    /// </summary>
    public required string Number { get; set; }

    /// <summary>
    /// Прикрепленный клиент
    /// </summary>
    public Client Client { get; set; } = default!;
    public int ClientId { get; set; }

    /// <summary>
    /// Дата поступления
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Статус подписания
    /// </summary>
    public Status Status { get; set; }

    /// <summary>
    /// Прикрепленные ресурсы отгрузки
    /// </summary>
    public List<ShipmentResource> ShipmentResources { get; set; } = default!;

}
