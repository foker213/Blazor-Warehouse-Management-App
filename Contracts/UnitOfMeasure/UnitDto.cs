using WarehouseManagement.Contracts.Enums;

namespace WarehouseManagement.Contracts.UnitOfMeasure;

public class UnitDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public State State { get; set; }
}
