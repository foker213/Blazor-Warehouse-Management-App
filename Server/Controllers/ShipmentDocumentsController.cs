using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Application.IServices;
using WarehouseManagement.Contracts;
using WarehouseManagement.Contracts.Shipment;

namespace WarehouseManagement.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class ShipmentDocumentsController : ControllerBase
{
    private readonly IShipmentDocumentService _shipmentDocumentService;

    public ShipmentDocumentsController(IShipmentDocumentService shipmentDocumentService)
    {
        _shipmentDocumentService = shipmentDocumentService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ShipmentDocumentDto>>> GetAll(CancellationToken ct = default)
    {
        var documents = await _shipmentDocumentService.GetAll(ct);
        return Ok(documents);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ShipmentDocumentDto>> GetById(int id, CancellationToken ct = default)
    {
        var result = await _shipmentDocumentService.GetBy(id, ct);

        if (result.IsError && result.FirstError.Type == ErrorType.NotFound)
            return NotFound();

        if (result.IsError)
            return Problem(result.FirstError.Description);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ShipmentDocumentDto shipment, CancellationToken ct = default)
    {
        var result = await _shipmentDocumentService.CreateAsync(shipment, ct);

        if (result.IsError && result.FirstError.Type == ErrorType.Validation)
            return BadRequest(result.FirstError.Description);

        if (result.IsError && result.FirstError.Type == ErrorType.Conflict)
            return Conflict(result.FirstError.Description);

        if (result.IsError)
            return Problem(result.FirstError.Description);

        return CreatedAtAction(nameof(GetById), new { id = shipment.Id }, shipment);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] ShipmentDocumentDto shipment, CancellationToken ct = default)
    {
        return StatusCode(StatusCodes.Status501NotImplemented, "Update operation is not yet implemented");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        return StatusCode(StatusCodes.Status501NotImplemented, "Delete operation is not yet implemented");
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> ChangeStatus(int id, CancellationToken ct = default)
    {
        return StatusCode(StatusCodes.Status501NotImplemented, "ChangeStatus operation is not yet implemented");
    }

    [HttpPost("filter")]
    public async Task<ActionResult<List<ShipmentDocumentDto>>> Filter([FromBody] FilterDto filter, CancellationToken ct = default)
    {
        var documents = await _shipmentDocumentService.FilterAsync(filter, ct);
        return Ok(documents);
    }
}