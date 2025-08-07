using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Seeds;

internal sealed class ReceiptsResource
{
    public static List<ReceiptResource> Get()
    {
        return new()
        {
            new ReceiptResource
            {
                ReceiptDocumentId = 1,
                ResourceId = 1,
                UnitOfMeasureId = 1,
                Quantity = 15
            },
            new ReceiptResource
            {
                ReceiptDocumentId = 1,
                ResourceId = 2,
                UnitOfMeasureId = 2,
                Quantity = 10
            },
            new ReceiptResource
            {
                ReceiptDocumentId = 2,
                ResourceId = 3,
                UnitOfMeasureId = 3,
                Quantity = 100
            },
            new ReceiptResource
            {
                ReceiptDocumentId = 2,
                ResourceId = 4,
                UnitOfMeasureId = 4,
                Quantity = 3
            },
            new ReceiptResource
            {
                ReceiptDocumentId = 3,
                ResourceId = 5,
                UnitOfMeasureId = 5,
                Quantity = 24
            },
            new ReceiptResource
            {
                ReceiptDocumentId = 3,
                ResourceId = 6,
                UnitOfMeasureId = 6,
                Quantity = 11
            }
        };
    }
}
