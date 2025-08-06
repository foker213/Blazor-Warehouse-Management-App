using ErrorOr;
using Mapster;
using WarehouseManagement.Application.IRepositories;
using WarehouseManagement.Application.IServices;
using WarehouseManagement.Contracts.UnitOfMeasure;
using WarehouseManagement.Domain.Enums;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Application.Services;

internal sealed class UnitOfMeasureService : IUnitOfMeasureService
{
    private readonly IUnitOfMeasureRepository _unitOfMeasureRepository;

    public UnitOfMeasureService(IUnitOfMeasureRepository unitOfMeasureRepository)
    {
        _unitOfMeasureRepository = unitOfMeasureRepository;
    }
    public async Task<List<UnitDto>> GetAll(CancellationToken ct = default)
    {
        List<UnitOfMeasure> items = await _unitOfMeasureRepository.GetAll(ct);

        return items.Adapt<List<UnitDto>>();
    }

    public async Task<ErrorOr<UnitDto>> GetBy(int id, CancellationToken ct = default)
    {
        UnitOfMeasure? unit = await _unitOfMeasureRepository.GetBy(id, ct);

        if (unit is null)
            return ErrorOr<UnitDto>.From(new List<Error> { Error.NotFound() });

        return unit.Adapt<UnitDto>();
    }

    public async Task<ErrorOr<Created>> CreateAsync(UnitCreateDto unit, CancellationToken ct = default)
    {
        Error? error = await ValidateUnit(unit.Name, ct);
        if (error is not null)
            return ErrorOr<Created>.From(new List<Error> { error.Value });

        ErrorOr<Created> result = await _unitOfMeasureRepository.CreateAsync(unit.Adapt<UnitOfMeasure>(), ct);

        if (result.IsError)
            return result.FirstError;

        return result;
    }

    public async Task<ErrorOr<Updated>> UpdateAsync(UnitUpdateDto unit, CancellationToken ct = default)
    {
        Error? error = await ValidateUnit(unit.Name, ct);
        if (error is not null)
            return ErrorOr<Updated>.From(new List<Error> { error.Value });

        ErrorOr<Updated> result = await _unitOfMeasureRepository.UpdateAsync(unit.Adapt<UnitOfMeasure>(), ct);

        if (result.IsError)
            return result.FirstError;

        return result;
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(int id, CancellationToken ct = default)
    {
        UnitOfMeasure? unit = await _unitOfMeasureRepository.GetBy(id);

        if (unit == null)
            return ErrorOr<Deleted>.From(new List<Error> { Error.NotFound() });

        if (unit.ShipmentResource is not null || unit.ReceiptResource is not null)
            return ErrorOr<Deleted>.From(new List<Error> { Error.Conflict("Deleted", "Невозможно удалить: единица измерения используется") });

        ErrorOr<Deleted> result = await _unitOfMeasureRepository.DeleteAsync(unit, ct);

        if (result.IsError)
            return result.FirstError;

        return result;
    }

    public async Task<ErrorOr<Updated>> ChangeStateAsync(int id, CancellationToken ct = default)
    {
        UnitOfMeasure? resource = await _unitOfMeasureRepository.GetBy(id, ct);
        if (resource == null)
            return Error.NotFound("ClientNotFound", "Клиент не найден");

        if (resource.State == State.InWork)
            resource.State = State.InArchive;
        else
            resource.State = State.InWork;

        return await _unitOfMeasureRepository.ChangeStateAsync(resource, ct);
    }

    private async Task<Error?> ValidateUnit(string name, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Error.Validation("Name", "Поле наименования должно быть заполнено");
        }

        UnitOfMeasure? response = await _unitOfMeasureRepository.GetByName(name, ct);

        if (response is not null)
            return Error.Conflict("Name", "Единица измерения с таким наименованием уже существует");

        return default;
    }
}