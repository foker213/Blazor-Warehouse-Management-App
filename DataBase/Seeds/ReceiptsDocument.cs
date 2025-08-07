using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Seeds;

internal sealed class ReceiptsDocument
{
    public static List<ReceiptDocument> Get()
    {
        return new()
        {
            new ReceiptDocument
            {
                Number = "Новая поставка №1",
                Date = DateOnly.FromDateTime(DateTime.Now)
            },
            new ReceiptDocument
            {
                Number = "Новая поставка №2",
                Date = DateOnly.FromDateTime(DateTime.Now)
            },
            new ReceiptDocument
            {
                Number = "Новая поставка №3",
                Date = DateOnly.FromDateTime(DateTime.Now)
            }
        };
    }
}
