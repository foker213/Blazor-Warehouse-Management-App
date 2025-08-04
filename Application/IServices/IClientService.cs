using ErrorOr;
using WarehouseManagement.Contracts.Client;

namespace WarehouseManagement.Application.IServices;

public interface IClientService
{
    Task<List<ClientDto>> GetAll(CancellationToken ct = default);
    Task<ErrorOr<ClientDto>> GetBy(int id, CancellationToken ct = default);
    Task<ErrorOr<Created>> CreateAsync(ClientDto client, CancellationToken ct = default);
    Task<ErrorOr<Updated>> UpdateAsync(ClientDto client, CancellationToken ct = default);
    Task<ErrorOr<Deleted>> DeleteAsync(int id, CancellationToken ct = default);
    Task<ErrorOr<Updated>> ChangeStateAsync(int id, CancellationToken ct = default);
}