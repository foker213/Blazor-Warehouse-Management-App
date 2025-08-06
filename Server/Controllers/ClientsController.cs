using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Application.IServices;
using WarehouseManagement.Contracts;
using WarehouseManagement.Contracts.Client;
using WarehouseManagement.Contracts.Resource;

namespace WarehouseManagement.Server.Controllers;

[ApiController]
[Route(Routes.Clients.Api)]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ClientDto>>> GetAll(CancellationToken ct = default)
    {
        var clients = await _clientService.GetAll(ct);
        return Ok(clients);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClientDto>> GetById(int id, CancellationToken ct = default)
    {
        var result = await _clientService.GetBy(id, ct);

        if (result.IsError && result.FirstError.Type == ErrorType.NotFound)
            return NotFound();

        if (result.IsError)
            return Problem(result.FirstError.Description);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ClientCreateDto client, CancellationToken ct = default)
    {
        var result = await _clientService.CreateAsync(client, ct);

        if (result.IsError && result.FirstError.Type == ErrorType.Validation)
            return BadRequest(result.FirstError.Description);

        if (result.IsError && result.FirstError.Type == ErrorType.Conflict)
            return Conflict(result.FirstError.Description);

        if (result.IsError)
            return Problem(result.FirstError.Description);

        return Ok();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromBody] ClientUpdateDto client, CancellationToken ct = default)
    {
        var result = await _clientService.UpdateAsync(client, ct);

        if (result.IsError && result.FirstError.Type == ErrorType.Validation)
            return BadRequest(result.FirstError.Description);

        if (result.IsError && result.FirstError.Type == ErrorType.NotFound)
            return NotFound();

        if (result.IsError)
            return Problem(result.FirstError.Description);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        var result = await _clientService.DeleteAsync(id, ct);

        if (result.IsError && result.FirstError.Type == ErrorType.NotFound)
            return NotFound();

        if (result.IsError && result.FirstError.Type == ErrorType.Conflict)
            return Conflict(result.FirstError.Description);

        if (result.IsError)
            return Problem(result.FirstError.Description);

        return NoContent();
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> ChangeState(int id, CancellationToken ct = default)
    {
        var result = await _clientService.ChangeStateAsync(id, ct);

        if (result.IsError && result.FirstError.Type == ErrorType.NotFound)
            return NotFound();

        if (result.IsError)
            return Problem(result.FirstError.Description);

        return NoContent();
    }
}