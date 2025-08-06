using System.ComponentModel.DataAnnotations;
using WarehouseManagement.Contracts.Enums;

namespace WarehouseManagement.Contracts.Client;

public class ClientUpdateDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Наименование обязательно")]
    [StringLength(40, MinimumLength = 5, ErrorMessage = "Наименование должно быть от 5 до 40 символов")]
    public string Name { get; set; } = default!;

    [Required(ErrorMessage = "Адресс должен быть заполнен")]
    [StringLength(120, MinimumLength = 10, ErrorMessage = "Адрес должен быть от 10 до 120 символов")]
    public string Adress { get; set; } = default!;
    public State State { get; set; }
}
