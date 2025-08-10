using ErrorOr;
using Mapster;
using WarehouseManagement.Application.IRepositories;
using WarehouseManagement.Application.IServices;
using WarehouseManagement.Contracts.Resource;
using WarehouseManagement.Domain.Enums;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Application.Services;

internal sealed class ResourceService : IResourceService
{
    private readonly IResourceRepository _resourceRepository;

    public ResourceService(IResourceRepository resourceRepository)
    {
        _resourceRepository = resourceRepository;
    }

    public async Task<List<ResourceDto>> GetAll(CancellationToken ct = default)
    {
        List<Resource> items = await _resourceRepository.GetAllAsync(ct);

        return items.Adapt<List<ResourceDto>>();
    }

    public async Task<ErrorOr<ResourceDto>> GetBy(int id, CancellationToken ct = default)
    {
        Resource? resource = await _resourceRepository.GetByAsync(id, ct);

        if (resource is null)
            return ErrorOr<ResourceDto>.From(new List<Error> { Error.NotFound() });

        return resource.Adapt<ResourceDto>();
    }

    public async Task<ErrorOr<Created>> CreateAsync(ResourceCreateDto resource, CancellationToken ct = default)
    {
        Error? error = await ValidateResource(resource.Name, ct);
        if (error is not null)
            return ErrorOr<Created>.From(new List<Error> { error.Value });

        await _resourceRepository.CreateAsync(resource.Adapt<Resource>(), ct);

        return Result.Created;
    }

    public async Task<ErrorOr<Updated>> UpdateAsync(ResourceUpdateDto resource, CancellationToken ct = default)
    {
        Error? error = await ValidateResource(resource.Name, ct);
        if (error is not null)
            return ErrorOr<Updated>.From(new List<Error> { error.Value });

        await _resourceRepository.UpdateAsync(resource.Adapt<Resource>(), ct);

        return Result.Updated;
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(int id, CancellationToken ct = default)
    {
        Resource? resource = await _resourceRepository.GetByAsync(id);

        if (resource == null)
            return ErrorOr<Deleted>.From(new List<Error> { Error.NotFound() });

        if (resource.ShipmentResources is not null || resource.ReceiptResources is not null)
            return ErrorOr<Deleted>.From(new List<Error> { Error.Conflict("Deleted", "Невозможно удалить: ресурс используется") });

        await _resourceRepository.DeleteAsync(resource, ct);

        return Result.Deleted;
    }

    public async Task<ErrorOr<Updated>> ChangeStateAsync(int id, CancellationToken ct = default)
    {
        Resource? resource = await _resourceRepository.GetByAsync(id, ct);
        if (resource == null)
            return Error.NotFound("DocumentNotFound", "Документ не найден");

        if (resource.State == State.InWork)
            resource.State = State.InArchive;
        else
            resource.State = State.InWork;

        return await _resourceRepository.ChangeStateAsync(resource, ct);
    }

    private async Task<Error?> ValidateResource(string name, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Error.Validation("Name", "Поле наименования должно быть заполнено");
        }

        Resource? response = await _resourceRepository.GetByName(name, ct);

        if (response is not null)
            return Error.Conflict("Name", "Ресурс с таким наименованием уже существует");

        return default;
    }
}
