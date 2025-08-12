using ErrorOr;
using WarehouseManagement.Domain.Enums;

namespace WarehouseManagement.Domain.Models;

public class ShipmentDocument : Entity
{
    /// <summary>
    /// Номер поступления
    /// </summary>
    public string Number { get; private set; } = default!;

    /// <summary>
    /// Прикрепленный клиент
    /// </summary>
    public Client? Client { get; private set; } = default!;

    /// <summary>
    /// Id прикрепленного клиента
    /// </summary>
    public int ClientId { get; private set; }

    /// <summary>
    /// Дата поступления
    /// </summary>
    public DateOnly Date { get; private set; }

    /// <summary>
    /// Статус отгрузки
    /// </summary>
    public Status Status { get; private set; }

    private readonly List<ShipmentResource> _shipmentResources = new();

    /// <summary>
    /// Прикрепленные ресурсы отгрузки
    /// </summary>
    public IReadOnlyCollection<ShipmentResource> ShipmentResources => _shipmentResources.AsReadOnly();

    protected ShipmentDocument() { }

    public static ErrorOr<ShipmentDocument> Create(string number, DateOnly date, int clientId, ShipmentDocument? existingDocument)
    {
        if (string.IsNullOrWhiteSpace(number))
            return Error.Validation("NumberRequired", "Номер документа обязателен");

        if (existingDocument is not null)
            return Error.Validation("NumberDuplicate", "Документ с таким номером уже существует");

        return new ShipmentDocument
        {
            Number = number,
            Date = date,
            ClientId = clientId,
            Status = Status.NotSigned
        };
    }

    public void Update(string number, DateOnly date, int clientId, bool isChangeStatus = false)
    {
        Number = number;
        Date = date;
        ClientId = clientId;
        Client = null;
        
        if(isChangeStatus)
            Status = Status == Status.NotSigned ? Status.Signed : Status.NotSigned;
    }

    public ErrorOr<ShipmentResource> AddResource(int resourceId, int unitId, int quantity, int id = 0)
    {
        if (quantity < 0)
            return Error.Validation("InvalidQuantity", "Количество должно быть положительным");

        ShipmentResource? existingResource = _shipmentResources.FirstOrDefault(r =>
            r.Id == id && r.Id != 0);

        if (existingResource != null)
        {
            existingResource.ChangeQuantity(quantity);
        }
        else
        {
            ErrorOr<ShipmentResource> receiptResource = ShipmentResource.Create(resourceId, unitId, quantity, this.Id);
            if (receiptResource.IsError)
                return receiptResource.Errors;

            existingResource = receiptResource.Value;
            _shipmentResources.Add(existingResource);
        }

        return existingResource;
    }

    public void RemoveResources(List<ShipmentResource> deletingResources)
    {
        if (deletingResources is null || deletingResources.Count() == 0)
            return;

        foreach (ShipmentResource resource in deletingResources)
        {
            _shipmentResources.Remove(resource);
        }
    }
}
