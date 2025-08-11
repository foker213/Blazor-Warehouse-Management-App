using System.ComponentModel.DataAnnotations;
using WarehouseManagement.Contracts.Enums;

namespace WarehouseManagement.Contracts.Receipt;

public class ReceiptDocumentCreateDto
{
    [Required(ErrorMessage = "Номер обязателен")]
    [StringLength(40, ErrorMessage = "Номер должен быть до 40 символов")]
    public string Number { get; set; } = default!;
    public DateTime Date { get; set; }
    public Status Status { get; set; } = Status.NotSigned;
    public List<ReceiptResourceCreateDto> ReceiptResources { get; set; } = new();
}
