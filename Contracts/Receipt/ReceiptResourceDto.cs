using System.ComponentModel.DataAnnotations;
using WarehouseManagement.Contracts.Resource;
using WarehouseManagement.Contracts.UnitOfMeasure;

namespace WarehouseManagement.Contracts.Receipt;

public class ReceiptResourceDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ресурс должен быть выбрана")]
    public required ResourceDto Resource { get; set; }

    [Required(ErrorMessage = "Единица измерения должна быть выбрана")]
    public required UnitDto UnitOfMeasure { get; set; }

    [Required(ErrorMessage = "Количество не задано")]
    public int Quantity { get; set; }
}
