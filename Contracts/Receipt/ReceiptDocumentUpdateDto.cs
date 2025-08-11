using System.ComponentModel.DataAnnotations;
using WarehouseManagement.Contracts.Enums;

namespace WarehouseManagement.Contracts.Receipt;

public class ReceiptDocumentUpdateDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Номер обязателен")]
    [StringLength(40, MinimumLength = 5, ErrorMessage = "Номер должен быть от 5 до 40 символов")]
    public string Number { get; set; } = default!;
    public DateTime Date { get; set; }
    public Status Status { get; set; }
    public List<ReceiptResourceUpdateDto> ReceiptResources { get; set; } = new();
}
