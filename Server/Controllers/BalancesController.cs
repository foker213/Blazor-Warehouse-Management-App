using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Application.IServices;
using WarehouseManagement.Contracts;
using WarehouseManagement.Contracts.Balance;

namespace WarehouseManagement.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class BalancesController : ControllerBase
{
    private readonly IBalanceService _balanceService;

    public BalancesController(IBalanceService balanceService)
    {
        _balanceService = balanceService;
    }

    [HttpGet]
    public async Task<ActionResult<List<BalanceDto>>> GetAll(CancellationToken ct = default)
    {
        var balances = await _balanceService.GetAll(ct);
        return Ok(balances);
    }

    [HttpPost("filter")]
    public async Task<ActionResult<List<BalanceDto>>> Filter([FromBody] FilterDto filter, CancellationToken ct = default)
    {
        var balances = await _balanceService.FilterAsync(filter, ct);
        return Ok(balances);
    }
}