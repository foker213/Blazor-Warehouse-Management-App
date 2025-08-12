using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Seeds;

internal sealed class ReceiptResources
{
    public static List<ReceiptResource> Get()
    {
        var resources = new List<ReceiptResource>();

        var resource1 = ReceiptResource.Create(1, 1, 15, 1);
        resources.Add(resource1.Value);

        var resource2 = ReceiptResource.Create(1, 2, 10, 1);
        resources.Add(resource2.Value);

        var resource3 = ReceiptResource.Create(2, 3, 100, 2);
        resources.Add(resource3.Value);

        var resource4 = ReceiptResource.Create(2, 4, 3, 2);
        resources.Add(resource4.Value);

        var resource5 = ReceiptResource.Create(3, 5, 24, 3);
        resources.Add(resource5.Value);

        var resource6 = ReceiptResource.Create(3, 6, 11, 3);
        resources.Add(resource6.Value);

        return resources;
    }
}