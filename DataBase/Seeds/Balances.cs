using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Seeds;

internal sealed class Balances
{
    public static List<Balance> Get()
    {
        return new()
        {
            new Balance
            {
                ResourceId = 1,
                UnitOfMeasureId = 1,
                Quantity = 15
            },
            new Balance
            {
                ResourceId = 2,
                UnitOfMeasureId = 2,
                Quantity = 10
            },
            new Balance
            {
                ResourceId = 3,
                UnitOfMeasureId = 3,
                Quantity = 100
            },
            new Balance
            {
                ResourceId = 4,
                UnitOfMeasureId = 4,
                Quantity = 3
            },
            new Balance
            {
                ResourceId = 5,
                UnitOfMeasureId = 5,
                Quantity = 24
            },
            new Balance
            {
                ResourceId = 6,
                UnitOfMeasureId = 6,
                Quantity = 11
            }
        };
    }
}
