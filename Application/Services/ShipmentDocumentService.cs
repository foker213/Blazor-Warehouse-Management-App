using ErrorOr;
using Mapster;
using WarehouseManagement.Application.IRepositories;
using WarehouseManagement.Application.IServices;
using WarehouseManagement.Contracts;
using WarehouseManagement.Contracts.Shipment;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Application.Services;

internal sealed class ShipmentDocumentService : IShipmentDocumentService
{
    private readonly IShipmentDocumentRepository _documentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBalanceService _balanceService;

    public ShipmentDocumentService(IShipmentDocumentRepository documentRepository, IUnitOfWork unitOfWork, IBalanceService balanceService)
    {
        _documentRepository = documentRepository;
        _unitOfWork = unitOfWork;
        _balanceService = balanceService;
    }

    public async Task<List<ShipmentDocumentDto>> GetAll(CancellationToken ct = default)
    {
        List<ShipmentDocument> items = await _documentRepository.GetAllAsync(ct);

        return items.Adapt<List<ShipmentDocumentDto>>();
    }

    public async Task<ErrorOr<ShipmentDocumentDto>> GetBy(int id, CancellationToken ct = default)
    {
        ShipmentDocument? client = await _documentRepository.GetByAsync(id, ct);

        if (client is null)
            return ErrorOr<ShipmentDocumentDto>.From(new List<Error> { Error.NotFound() });

        return client.Adapt<ShipmentDocumentDto>();
    }

    public async Task<ErrorOr<Created>> CreateAsync(ShipmentDocumentDto shipment, CancellationToken ct = default)
    {
        Error? error = await ValidateShipment(shipment, ct);

        if (error is not null)
            return ErrorOr<Created>.From(new List<Error> { error.Value });

        return Result.Created;
    }

    public Task<ErrorOr<Updated>> UpdateAsync(ShipmentDocumentDto shipment, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Deleted>> DeleteAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<Updated>> ChangeStatusAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<List<ShipmentDocumentDto>> FilterAsync(FilterDto filter, CancellationToken ct = default)
    {
        List<ShipmentDocument> items = await _documentRepository.FilterAsync(filter, ct);

        return items.Adapt<List<ShipmentDocumentDto>>();
    }

    private async Task<Error?> ValidateShipment(ShipmentDocumentDto shipment, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(shipment.Number))
            return Error.Validation("NumberNull", "Поле номера должно быть заполнено");

        if (shipment.ShipmentResources is not null && shipment.ShipmentResources.Count > 0)
        {
            foreach (ShipmentResourceDto resource in shipment.ShipmentResources)
            {
                if (resource.Quantity < 0)
                {
                    return Error.Validation("NegativeBalance", "Количество ресурсов не может быть отрицательным");
                }
            }
        }
        else
            return Error.Validation("NegativeResource", "Документ не может не содержать ресурсов");

        ShipmentDocument? response = await _documentRepository.GetByNumber(shipment.Number, ct);

        if (response is not null && shipment.Id != response.Id)
            return Error.Conflict("NumberExist", "Поставка с таким номером уже существует");

        return default;
    }
}