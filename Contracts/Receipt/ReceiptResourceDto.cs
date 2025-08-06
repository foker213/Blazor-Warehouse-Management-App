using WarehouseManagement.Contracts.Resource;
using WarehouseManagement.Contracts.UnitOfMeasure;

namespace WarehouseManagement.Contracts.Receipt;

public class ReceiptResourceDto
{
    public int Id { get; set; }
    public required ResourceDto Resource { get; set; }
    public required UnitDto Unit { get; set; }
    public int Quantity { get; set; }
}
