using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Application.IServices;
using WarehouseManagement.Contracts.UnitOfMeasure;

namespace WarehouseManagement.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class UnitsController : ControllerBase
{
    private readonly IUnitOfMeasureService _unitService;

    public UnitsController(IUnitOfMeasureService unitService)
    {
        _unitService = unitService;
    }

    [HttpGet]
    public async Task<ActionResult<List<UnitOfMeasureDto>>> GetAll(CancellationToken ct = default)
    {
        var units = await _unitService.GetAll(ct);
        return Ok(units);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UnitOfMeasureDto>> GetById(int id, CancellationToken ct = default)
    {
        var result = await _unitService.GetBy(id, ct);

        if (result.IsError && result.FirstError.Type == ErrorType.NotFound)
            return NotFound();

        if (result.IsError)
            return Problem(result.FirstError.Description);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UnitOfMeasureDto unit, CancellationToken ct = default)
    {
        var result = await _unitService.CreateAsync(unit, ct);

        if (result.IsError && result.FirstError.Type == ErrorType.Validation)
            return BadRequest(result.FirstError.Description);

        if (result.IsError && result.FirstError.Type == ErrorType.Conflict)
            return Conflict(result.FirstError.Description);

        if (result.IsError)
            return Problem(result.FirstError.Description);

        return CreatedAtAction(nameof(GetById), new { id = unit.Id }, unit);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UnitOfMeasureDto unit, CancellationToken ct = default)
    {
        var result = await _unitService.UpdateAsync(unit, ct);

        if (result.IsError && result.FirstError.Type == ErrorType.Validation)
            return BadRequest(result.FirstError.Description);

        if (result.IsError && result.FirstError.Type == ErrorType.NotFound)
            return NotFound();

        if (result.IsError && result.FirstError.Type == ErrorType.Conflict)
            return Conflict(result.FirstError.Description);

        if (result.IsError)
            return Problem(result.FirstError.Description);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        var result = await _unitService.DeleteAsync(id, ct);

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
        var result = await _unitService.ChangeStateAsync(id, ct);

        if (result.IsError && result.FirstError.Type == ErrorType.NotFound)
            return NotFound();

        if (result.IsError)
            return Problem(result.FirstError.Description);

        return NoContent();
    }
}