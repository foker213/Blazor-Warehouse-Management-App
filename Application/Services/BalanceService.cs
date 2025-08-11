using ErrorOr;
using Mapster;
using WarehouseManagement.Application.IRepositories;
using WarehouseManagement.Application.IServices;
using WarehouseManagement.Contracts;
using WarehouseManagement.Contracts.Balance;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.Application.Services;

internal sealed class BalanceService : IBalanceService
{
    private readonly IBalanceRepository _balanceRepository;
    public BalanceService(IBalanceRepository balanceRepository)
    {
        _balanceRepository = balanceRepository;
    }

    public async Task<List<BalanceDto>> FilterAsync(FilterDto filter, CancellationToken ct)
    {
        List<Balance> items = await _balanceRepository.FilterAsync(filter, ct);

        return items.Adapt<List<BalanceDto>>();
    }

    public async Task<List<BalanceDto>> GetAll(CancellationToken ct)
    {
        List<Balance> items = await _balanceRepository.GetAllAsync(ct);

        return items.Adapt<List<BalanceDto>>();
    }
}
