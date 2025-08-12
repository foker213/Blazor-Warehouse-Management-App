using ErrorOr;

namespace WarehouseManagement.Domain.Models;

public class Balance : Entity
{
    /// <summary>
    /// Прикрепленный ресурс
    /// </summary>
    public Resource Resource { get; private set; } = default!;

    /// <summary>
    /// Id прикрепленного ресурса
    /// </summary>
    public int ResourceId { get; private set; }

    /// <summary>
    /// Прикрепленная единица измерения
    /// </summary>
    public UnitOfMeasure UnitOfMeasure { get; private set; } = default!;

    /// <summary>
    /// Id прикрепленной единицы измерения
    /// </summary>
    public int UnitOfMeasureId { get; private set; }

    /// <summary>
    /// Количество
    /// </summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// Проверка существования в базе
    /// </summary>
    private bool _existInDb { get; set; } = true;

    protected Balance() { }

    public static ErrorOr<Balance> Create(int resourceId, int unitId, int initialQuantity)
    {
        if (initialQuantity < 0)
            return Error.Validation("NegativeQuantity", "Количество не может быть отрицательным");

        return new Balance
        {
            ResourceId = resourceId,
            UnitOfMeasureId = unitId,
            Quantity = initialQuantity,
            _existInDb = false
        };
    }

    public ErrorOr<Success> ChangeQueantity(int quantity)
    {
        if (quantity < 0)
            return Error.Validation("NegativeQuantity", $"На складе станет недостаточно ресурсов");

        Quantity = quantity;
        return Result.Success;
    }

    public ErrorOr<Success> Increase(int quantity)
    {
        if (quantity <= 0)
            return Error.Validation("InvalidQuantity", "Сумма увеличения должна быть положительной");

        Quantity += quantity;
        return Result.Success;
    }

    public ErrorOr<Success> Decrease(int quantity)
    {
        if (quantity <= 0)
            return Error.Validation("InvalidQuantity", "Сумма уменьшения должна быть положительной");
        if (Quantity < quantity)
            return Error.Conflict("InsufficientBalance", "Недостаточно ресурсов на балансе");

        Quantity -= quantity;
        return Result.Success;
    }

    public bool CheckExistInDb()
        => _existInDb;
}
