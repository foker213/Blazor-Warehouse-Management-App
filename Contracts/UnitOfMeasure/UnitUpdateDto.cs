using System.ComponentModel.DataAnnotations;
using WarehouseManagement.Contracts.Enums;

namespace WarehouseManagement.Contracts.UnitOfMeasure;

public class UnitUpdateDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Наименование обязательно")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Наименование должно быть от 1 до 40 символов")]
    public string Name { get; set; } = default!;
    public State State { get; set; }
}
