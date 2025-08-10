using ErrorOr;
using Mapster;
using System.IO.Pipelines;
using WarehouseManagement.Application.IRepositories;
using WarehouseManagement.Application.IServices;
using WarehouseManagement.Contracts.Client;
using WarehouseManagement.Domain.Enums;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Application.Services;

internal sealed class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;

    public ClientService(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<List<ClientDto>> GetAll(CancellationToken ct = default)
    {
        List<Client> items = await _clientRepository.GetAllAsync(ct);

        return items.Adapt<List<ClientDto>>();
    }

    public async Task<ErrorOr<ClientDto>> GetBy(int id, CancellationToken ct = default)
    {
        Client? client = await _clientRepository.GetByAsync(id, ct);

        if (client is null)
            return ErrorOr<ClientDto>.From(new List<Error> { Error.NotFound() });

        return client.Adapt<ClientDto>();
    }

    public async Task<ErrorOr<Created>> CreateAsync(ClientCreateDto client, CancellationToken ct = default)
    {
        Error? error = await ValidateClient(client.Name);
        if (error is not null)
            return ErrorOr<Created>.From(new List<Error> { error.Value });

        await _clientRepository.CreateAsync(client.Adapt<Client>(), ct);

        return Result.Created;
    }

    public async Task<ErrorOr<Updated>> UpdateAsync(ClientUpdateDto client, CancellationToken ct = default)
    {
        Error? error = await ValidateClient(client.Name, client.Id, ct);
        if (error is not null)
            return ErrorOr<Updated>.From(new List<Error> { error.Value });

        await _clientRepository.UpdateAsync(client.Adapt<Client>(), ct);

        return Result.Updated;
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(int id, CancellationToken ct = default)
    {
        Client? client = await _clientRepository.GetByAsync(id);

        if (client == null)
            return ErrorOr<Deleted>.From(new List<Error> { Error.NotFound() });

        if(client.ShipmentDocument is not null)
            return ErrorOr<Deleted>.From(new List<Error> { Error.Conflict("Deleted", "Невозможно удалить: клиент используется") });

        await _clientRepository.DeleteAsync(client, ct);

        return Result.Deleted;
    }

    public async Task<ErrorOr<Updated>> ChangeStateAsync(int id, CancellationToken ct = default)
    {
        Client? resource = await _clientRepository.GetByAsync(id, ct);
        if (resource == null)
            return Error.NotFound("ClientNotFound", "Клиент не найден");

        if (resource.State == State.InWork)
            resource.State = State.InArchive;
        else
            resource.State = State.InWork;

        return await _clientRepository.ChangeStateAsync(resource, ct);
    }

    private async Task<Error?> ValidateClient(string name, int id = 0, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Error.Validation("Name", "Поле наименования должно быть заполнено");
        }

        Client? response = await _clientRepository.GetByName(name, ct);

        if (response is not null && response.Id != id)
            return Error.Conflict("Name", "Клиент с таким наименованием уже существует");

        return default;
    }
}
