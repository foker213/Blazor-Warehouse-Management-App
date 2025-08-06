using WarehouseManagement.Contracts.Resource;
using WarehouseManagement.Contracts.UnitOfMeasure;

namespace WarehouseManagement.Contracts.Balance;

public class BalanceDto
{
    public int Id { get; set; }
    public required ResourceDto Resource { get; set; }
    public required UnitDto Unit { get; set; }
}
