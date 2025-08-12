using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Seeds;

internal sealed class Balances
{
    public static List<Balance> Get()
    {
        List<Balance> balances = new();

        var balance1 = Balance.Create(1, 1, 10);
        balances.Add(balance1.Value);

        var balance2 = Balance.Create(1, 2, 5);
        balances.Add(balance2.Value);

        var balance3 = Balance.Create(2, 3, 95);
        balances.Add(balance3.Value);

        var balance4 = Balance.Create(2, 4, 1);
        balances.Add(balance4.Value);

        var balance5 = Balance.Create(3, 5, 19);
        balances.Add(balance5.Value);

        var balance6 = Balance.Create(3, 6, 6);
        balances.Add(balance6.Value);

        return balances;
    }
}
