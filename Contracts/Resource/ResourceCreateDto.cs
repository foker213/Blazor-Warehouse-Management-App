using System.ComponentModel.DataAnnotations;
using WarehouseManagement.Contracts.Enums;

namespace WarehouseManagement.Contracts.Resource;

public class ResourceCreateDto
{
    [Required(ErrorMessage = "Наименование обязательно")]
    [StringLength(40, ErrorMessage = "Наименование должно быть до 40 символов")]
    public string Name { get; set; } = default!;
    public State State { get; set; }
}
