using WarehouseManagement.Contracts.Enums;

namespace WarehouseManagement.Contracts.Resource;

public class ResourceDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public State State { get; set; }
}
