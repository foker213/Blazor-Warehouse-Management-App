using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Seeds;

internal sealed class ShipmentDocuments
{
    public static List<ShipmentDocument> Get()
    {
        List<ShipmentDocument> result = new();

        var doc1 = ShipmentDocument.Create("Новая отгрузка №1", DateOnly.FromDateTime(DateTime.Now), 1, null);
        doc1.Value.Update("Новая отгрузка №1", DateOnly.FromDateTime(DateTime.Now), 1, true);
        result.Add(doc1.Value);

        var doc2 = ShipmentDocument.Create("Новая отгрузка №2", DateOnly.FromDateTime(DateTime.Now), 2, null);
        doc2.Value.Update("Новая отгрузка №2", DateOnly.FromDateTime(DateTime.Now), 2, true);
        result.Add(doc2.Value);

        var doc3 = ShipmentDocument.Create("Новая отгрузка №3", DateOnly.FromDateTime(DateTime.Now), 3, null);
        doc3.Value.Update("Новая отгрузка №3", DateOnly.FromDateTime(DateTime.Now), 3, true);
        result.Add(doc3.Value);

        return result;
    }
}
