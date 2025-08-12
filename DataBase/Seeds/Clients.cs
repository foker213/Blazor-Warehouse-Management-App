using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Seeds;

internal sealed class Clients
{
    public static List<Client> Get()
    {
        List<Client> clients = new();

        for (int i = 1; i <= 3; i++)
        {
            Client clientItem = new()
            {
                Name = "Рабочий клиент №" + i.ToString(),
                Adress = "Россия, г. Пенза, ул. Реалистов, д. 9Б",
                State = Domain.Enums.State.InWork
            };

            clients.Add(clientItem);
        }

        for (int i = 1; i <= 3; i++)
        {
            Client resourceItem = new()
            {
                Name = "Архивный клиент №" + i.ToString(),
                Adress = "Россия, г. Оренбург, ул. Двужального, д. 6А",
                State = Domain.Enums.State.InArchive
            };

            clients.Add(resourceItem);
        }

        return clients;
    }
}
