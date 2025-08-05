using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Application.IServices;
using WarehouseManagement.Contracts.Resource;

namespace WarehouseManagement.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class ResourcesController : ControllerBase
{
    private readonly IResourceService _resourceService;

    public ResourcesController(IResourceService resourceService)
    {
        _resourceService = resourceService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ResourceDto>>> GetAll(CancellationToken ct = default)
    {
        var resources = await _resourceService.GetAll(ct);
        return Ok(resources);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ResourceDto>> GetById(int id, CancellationToken ct = default)
    {
        var result = await _resourceService.GetBy(id, ct);

        if (result.IsError && result.FirstError.Type == ErrorType.NotFound)
            return NotFound();

        if (result.IsError)
            return Problem(result.FirstError.Description);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ResourceDto resource, CancellationToken ct = default)
    {
        var result = await _resourceService.CreateAsync(resource, ct);

        if (result.IsError && result.FirstError.Type == ErrorType.Validation)
            return BadRequest(result.FirstError.Description);

        if (result.IsError && result.FirstError.Type == ErrorType.Conflict)
            return Conflict(result.FirstError.Description);

        if (result.IsError)
            return Problem(result.FirstError.Description);

        return CreatedAtAction(nameof(GetById), new { id = resource.Id }, resource);
    }

    [HttpPut]
    public async Task<IActionResult> Update(ResourceDto resource, CancellationToken ct = default)
    {
        var result = await _resourceService.UpdateAsync(resource, ct);

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
        var result = await _resourceService.DeleteAsync(id, ct);

        if (result.IsError && result.FirstError.Type == ErrorType.NotFound)
            return NotFound();

        if (result.IsError && result.FirstError.Type == ErrorType.Conflict)
            return Conflict(result.FirstError.Description);

        if (result.IsError)
            return Problem(result.FirstError.Description);

        return NoContent();
    }

    [HttpPatch("{id:int}/state")]
    public async Task<IActionResult> ChangeState(int id, CancellationToken ct = default)
    {
        var result = await _resourceService.ChangeStateAsync(id, ct);

        if (result.IsError && result.FirstError.Type == ErrorType.NotFound)
            return NotFound();

        if (result.IsError)
            return Problem(result.FirstError.Description);

        return NoContent();
    }
}