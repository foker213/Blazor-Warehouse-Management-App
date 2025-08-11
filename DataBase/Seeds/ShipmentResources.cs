using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Seeds;

internal sealed class ShipmentResources
{
    public static List<ShipmentResource> Get() {
        var resources = new List<ShipmentResource>();

        var resource1 = ShipmentResource.Create(1, 1, 5, 1);
        resources.Add(resource1.Value);

        var resource2 = ShipmentResource.Create(1, 2, 5, 1);
        resources.Add(resource2.Value);

        var resource3 = ShipmentResource.Create(2, 3, 5, 2);
        resources.Add(resource3.Value);

        var resource4 = ShipmentResource.Create(2, 4, 2, 2);
        resources.Add(resource4.Value);

        var resource5 = ShipmentResource.Create(3, 5, 5, 3);
        resources.Add(resource5.Value);

        var resource6 = ShipmentResource.Create(3, 6, 5, 3);
        resources.Add(resource6.Value);

        return resources;
    }
}
