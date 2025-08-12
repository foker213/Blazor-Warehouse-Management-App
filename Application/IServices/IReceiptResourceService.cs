using ErrorOr;
using WarehouseManagement.Contracts.Receipt;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Application.IServices;

public interface IReceiptResourceService
{
    /// <summary>
    /// Обрабатывает создание ресурсов поставки
    /// </summary>
    ErrorOr<Success> ProcessResources(
        ReceiptDocument document,
        List<ReceiptResourceCreateDto> resources);

    /// <summary>
    /// Обрабатывает обновление ресурсов поставки
    /// </summary>
    ErrorOr<List<ReceiptResource>> ProcessResourceUpdates(
        ReceiptDocument document,
        List<ReceiptResourceUpdateDto> ReceiptResources);
}
