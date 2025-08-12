using System.ComponentModel.DataAnnotations;
using WarehouseManagement.Contracts.Enums;
using WarehouseManagement.Contracts.Receipt;

namespace WarehouseManagement.Contracts.Shipment;

public class ShipmentDocumentUpdateDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Номер обязателен")]
    [StringLength(40, ErrorMessage = "Номер должен быть до 40 символов")]
    public string Number { get; set; } = default!;
    public int ClientId { get; set; }
    public DateTime Date { get; set; }
    public Status Status { get; set; }
    public List<ShipmentResourceUpdateDto> ShipmentResources { get; set; } = new();
}
