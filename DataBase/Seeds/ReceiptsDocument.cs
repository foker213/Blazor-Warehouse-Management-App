using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Seeds;

internal sealed class ReceiptsDocument
{
    public static List<ReceiptDocument> Get()
    {
        List<ReceiptDocument> result = new();

        var doc1 = ReceiptDocument.Create("Новая поставка №1", DateOnly.FromDateTime(DateTime.Now), null);
        result.Add(doc1.Value);

        var doc2 = ReceiptDocument.Create("Новая поставка №2", DateOnly.FromDateTime(DateTime.Now), null);
        result.Add(doc2.Value);

        var doc3 = ReceiptDocument.Create("Новая поставка №3", DateOnly.FromDateTime(DateTime.Now), null);
        result.Add(doc3.Value);

        return result;
    }
}
