using ErrorOr;
using WarehouseManagement.Application.IServices;
using WarehouseManagement.Contracts.Receipt;
using WarehouseManagement.Domain.Models;

internal sealed class ReceiptResourceService : IReceiptResourceService
{
    public ErrorOr<Success> ProcessResources(
        ReceiptDocument document,
        List<ReceiptResourceCreateDto> resources)
    {
        foreach (ReceiptResourceCreateDto resourceDto in resources)
        {
            ErrorOr<Success> addResult = document.AddResource(
                resourceDto.ResourceId,
                resourceDto.UnitOfMeasureId,
                resourceDto.Quantity);

            if (addResult.IsError)
                return addResult.Errors.First();
        }

        return Result.Success;
    }

    public ErrorOr<List<ReceiptResource>> ProcessResourceUpdates(
        ReceiptDocument document,
        List<ReceiptResourceUpdateDto> receiptResources)
    {
        List<ReceiptResource> deletedReceiptsResource = new();

        if (receiptResources is null || receiptResources.Count == 0)
        {
            deletedReceiptsResource.AddRange(document.ReceiptResources);
            document.RemoveResources(deletedReceiptsResource);
        }
        else
        {
            deletedReceiptsResource = document.ReceiptResources
                .Where(r => !receiptResources.Any(k =>
                    k.Id == r.Id && k.Id != 0 &&
                    k.UnitOfMeasureId == r.UnitOfMeasureId &&
                    k.ResourceId == r.ResourceId))
                .ToList();

            document.RemoveResources(deletedReceiptsResource);

            foreach (ReceiptResourceUpdateDto receiptResource in receiptResources)
            {
                ErrorOr<Success> addResult = document.AddResource(
                    receiptResource.ResourceId,
                    receiptResource.UnitOfMeasureId,
                    receiptResource.Quantity,
                    receiptResource.Id);

                if (addResult.IsError)
                    return addResult.Errors.First();
            }
        }

        return deletedReceiptsResource;
    }
}