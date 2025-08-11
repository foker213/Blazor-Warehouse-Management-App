using ErrorOr;
using WarehouseManagement.Contracts.Client;

namespace WarehouseManagement.Application.IServices;

public interface IClientService
{
    Task<List<ClientDto>> GetAll(CancellationToken ct);
    Task<ErrorOr<ClientDto>> GetBy(int id, CancellationToken ct);
    Task<ErrorOr<Created>> CreateAsync(ClientCreateDto client, CancellationToken ct);
    Task<ErrorOr<Updated>> UpdateAsync(ClientUpdateDto client, CancellationToken ct);
    Task<ErrorOr<Deleted>> DeleteAsync(int id, CancellationToken ct);
    Task<ErrorOr<Updated>> ChangeStateAsync(int id, CancellationToken ct);
}