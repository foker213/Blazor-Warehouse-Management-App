using WarehouseManagement.Contracts.Enums;

namespace WarehouseManagement.Contracts.Client;

public class ClientDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Adress { get; set; }
    public State State { get; set; }
}
