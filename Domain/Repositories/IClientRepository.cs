using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Domain.Repositories;

public interface IClientRepository : IRepository<Client>
{
    Task ChangeStatusAsync(int id);
}