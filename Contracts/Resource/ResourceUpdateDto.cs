using System.ComponentModel.DataAnnotations;
using WarehouseManagement.Contracts.Enums;

namespace WarehouseManagement.Contracts.Resource;

public class ResourceUpdateDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Наименование обязательно")]
    [StringLength(40, MinimumLength = 5, ErrorMessage = "Наименование должно быть от 5 до 40 символов")]
    public string Name { get; set; } = default!;
    public State State { get; set; }
}
