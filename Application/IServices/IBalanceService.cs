using ErrorOr;
using WarehouseManagement.Contracts;
using WarehouseManagement.Contracts.Balance;
using WarehouseManagement.Contracts.Receipt;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Application.IServices;

public interface IBalanceService
{
    Task<List<BalanceDto>> GetAll(CancellationToken ct);
    Task<List<BalanceDto>> FilterAsync(FilterDto filter, CancellationToken ct);

    /// <summary>
    /// Обрабатывает соответствующие изменения баланса по добавленным ресурсам
    /// </summary>
    Task<ErrorOr<List<Balance>>> ProcessBalancesAsync(
        ReceiptDocument document,
        List<ReceiptResourceCreateDto> resources,
        CancellationToken ct);

    /// <summary>
    /// Обрабатывает соответствующие изменения баланса по обновленным ресурсам
    /// </summary>
    Task<ErrorOr<List<Balance>>> ProcessBalanceUpdatesAsync(
        ReceiptDocument document,
        List<ReceiptResource> existingResources,
        List<ReceiptResource> deletedResources,
        CancellationToken ct);

    /// <summary>
    /// Добавляет или обновляет балансы в DbContext
    /// </summary>
    void Add(List<Balance> balances);

    /// <summary>
    /// Добавляет, обновляет или удаляет балансы в DbContext
    /// </summary>
    void Update(List<Balance> balances);

    /// <summary>
    /// Обновляет или удаляет балансы в DbContext
    /// </summary>
    void Remove(List<Balance> balances);
}
