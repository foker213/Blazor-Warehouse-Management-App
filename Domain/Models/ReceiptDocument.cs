using ErrorOr;

namespace WarehouseManagement.Domain.Models;

public class ReceiptDocument : Entity
{
    /// <summary>
    /// Номер поступления
    /// </summary>
    public string Number { get; private set; } = default!;

    /// <summary>
    /// Дата поступления
    /// </summary>
    public DateOnly Date { get; private set; }

    private readonly List<ReceiptResource> _receiptResources = new();

    /// <summary>
    /// Прикрепленные ресурсы поступления
    /// </summary>
    public IReadOnlyCollection<ReceiptResource> ReceiptResources => _receiptResources.AsReadOnly();

    protected ReceiptDocument() { }

    public static ErrorOr<ReceiptDocument> Create(string number, DateOnly date, ReceiptDocument? existingDocument)
    {
        if (string.IsNullOrWhiteSpace(number))
            return Error.Validation("NumberRequired", "Номер документа обязателен");

        if (existingDocument is not null)
            return Error.Validation("NumberDuplicate", "Документ с таким номером уже существует");

        return new ReceiptDocument
        {
            Number = number,
            Date = date
        };
    }

    public void Update(string number, DateOnly date)
    {
        Number = number;
        Date = date;
    }

    public ErrorOr<Success> AddResource(int resourceId, int unitId, int quantity, int id = 0)
    {
        if (quantity <= 0)
            return Error.Validation("InvalidQuantity", "Количество должно быть положительным");

        ReceiptResource? existingResource = _receiptResources.FirstOrDefault(r =>
            r.Id == id && r.Id != 0);

        if (existingResource != null)
        {
            existingResource.ChangeQuantity(quantity);
        }
        else
        {
            ErrorOr<ReceiptResource> receiptResource = ReceiptResource.Create(resourceId, unitId, quantity, this);
            if (receiptResource.IsError)
                return receiptResource.Errors;

            _receiptResources.Add(receiptResource.Value);
        }

        return Result.Success;
    }

    public void RemoveResources(List<ReceiptResource> deletingResources)
    {
        if (deletingResources is null || deletingResources.Count() == 0)
            return;

        foreach (ReceiptResource resource in deletingResources)
        {
            _receiptResources.Remove(resource);
        }
    }
}
