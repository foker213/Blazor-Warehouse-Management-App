using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Seeds;

internal sealed class Resources
{
    public static List<Resource> Get()
    {
        List<Resource> resources = new List<Resource>();

        for (int i = 1; i <= 3; i++)
        {
            Resource resourceItem = new Resource()
            {
                Name = "Рабочий ресурс №" + i.ToString(),
                State = Domain.Enums.State.InWork
            };

            resources.Add(resourceItem);
        }

        for (int i = 1; i <= 3; i++)
        {
            Resource resourceItem = new Resource()
            {
                Name = "Архивный ресурс №" + i.ToString(),
                State = Domain.Enums.State.InArchive
            };

            resources.Add(resourceItem);
        }

        return resources;
    }
}