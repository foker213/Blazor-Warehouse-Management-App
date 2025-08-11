using ErrorOr;
using WarehouseManagement.Contracts.Receipt;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Application.IServices;

public interface IReceiptResourceService
{
    /// <summary>
    /// Обрабатывает создание ресурсов поставки и соответствующие изменения балансов
    /// </summary>
    Task<ErrorOr<List<Balance>>> ProcessResourcesAsync(
        ReceiptDocument document,
        List<ReceiptResourceCreateDto> resources,
        CancellationToken ct);

    /// <summary>
    /// Обрабатывает обновление ресурсов поставки и соответствующие изменения балансов
    /// </summary>
    Task<ErrorOr<List<Balance>>> ProcessResourceUpdatesAsync(
        ReceiptDocument document,
        List<ReceiptResource> existingResources,
        List<ReceiptResource> deletedResources,
        CancellationToken ct);

    /// <summary>
    /// Обрабатывает удаление ресурсов поставки и соответствующие изменения балансов
    /// </summary>
    Task<ErrorOr<List<Balance>>> ProcessResourceDeletionsAsync(
        List<ReceiptResource> deletedResources,
        CancellationToken ct);
}
