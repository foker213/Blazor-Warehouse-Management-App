using WarehouseManagement.Contracts.Enums;

namespace WarehouseManagement.Contracts.UnitOfMeasure;

public class UnitOfMeasureDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public State State { get; set; }
}
