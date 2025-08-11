using System.ComponentModel.DataAnnotations;
using WarehouseManagement.Contracts.Enums;

namespace WarehouseManagement.Contracts.Shipment;

public class ShipmentDocumentCreateDto
{
    [Required(ErrorMessage = "Номер обязателен")]
    [StringLength(40, ErrorMessage = "Номер должен быть до 40 символов")]
    public string Number { get; set; } = default!;

    [Required(ErrorMessage = "Клиент должен быть выбран")]
    public int ClientId { get; set; } = default!;
    public DateTime Date { get; set; }
    public Status Status { get; set; } = Status.NotSigned;

    [MinLength(1, ErrorMessage = "Список должен содержать хотя бы один элемент")]
    public List<ShipmentResourceCreateDto> ShipmentResources { get; set; } = default!;
}
