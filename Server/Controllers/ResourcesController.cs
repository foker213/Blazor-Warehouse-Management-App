using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Application.IServices;
using WarehouseManagement.Contracts;
using WarehouseManagement.Contracts.Resource;

namespace WarehouseManagement.Server.Controllers;

[ApiController]
[Route(Routes.Resources.Api)]
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
        List<ResourceDto> resources = await _resourceService.GetAll(ct);
        return Ok(resources);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ResourceDto>> GetById(int id, CancellationToken ct = default)
    {
        ErrorOr<ResourceDto> result = await _resourceService.GetBy(id, ct);

        if (result.IsError && result.FirstError.Type == ErrorType.NotFound)
            return NotFound();

        if (result.IsError)
            return Problem(result.FirstError.Description);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ResourceCreateDto resource, CancellationToken ct = default)
    {
        ErrorOr<Created> result = await _resourceService.CreateAsync(resource, ct);

        if (result.IsError && result.FirstError.Type == ErrorType.Validation)
            return BadRequest(result.FirstError.Description);

        if (result.IsError && result.FirstError.Type == ErrorType.Conflict)
            return Conflict(result.FirstError.Description);

        if (result.IsError)
            return Problem(result.FirstError.Description);

        return Ok();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(ResourceUpdateDto resource, CancellationToken ct = default)
    {
        ErrorOr<Updated> result = await _resourceService.UpdateAsync(resource, ct);

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
        ErrorOr<Deleted> result = await _resourceService.DeleteAsync(id, ct);

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
        ErrorOr<Updated> result = await _resourceService.ChangeStateAsync(id, ct);

        if (result.IsError && result.FirstError.Type == ErrorType.NotFound)
            return NotFound();

        if (result.IsError)
            return Problem(result.FirstError.Description);

        return NoContent();
    }
}