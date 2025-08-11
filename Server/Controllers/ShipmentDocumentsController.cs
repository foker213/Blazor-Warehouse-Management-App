using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Application.IServices;
using WarehouseManagement.Contracts;
using WarehouseManagement.Contracts.Shipment;

namespace WarehouseManagement.Server.Controllers;

[ApiController]
[Route(Routes.Shipments.Api)]
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
    public async Task<ActionResult<ShipmentDocumentUpdateDto>> GetById(int id, CancellationToken ct = default)
    {
        var result = await _shipmentDocumentService.GetBy(id, ct);

        if (result.IsError && result.FirstError.Type == ErrorType.NotFound)
            return NotFound();

        if (result.IsError)
            return Problem(result.FirstError.Description);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ShipmentDocumentCreateDto shipment, CancellationToken ct = default)
    {
        var result = await _shipmentDocumentService.CreateAsync(shipment, ct);

        if (result.IsError && result.FirstError.Type == ErrorType.Validation)
            return BadRequest(result.FirstError.Description);

        if (result.IsError && result.FirstError.Type == ErrorType.Conflict)
            return Conflict(result.FirstError.Description);

        if (result.IsError)
            return Problem(result.FirstError.Description);

        return NoContent();
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] ShipmentDocumentUpdateDto shipment, CancellationToken ct = default)
    {
        var result = await _shipmentDocumentService.UpdateAsync(shipment, ct);

        if (result.IsError && result.FirstError.Type == ErrorType.Validation)
            return BadRequest(result.FirstError.Description);

        if (result.IsError && result.FirstError.Type == ErrorType.Conflict)
            return Conflict(result.FirstError.Description);

        if (result.IsError)
            return Problem(result.FirstError.Description);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        var result = await _shipmentDocumentService.DeleteAsync(id, ct);

        if (result.IsError && result.FirstError.Type == ErrorType.NotFound)
            return NotFound();

        if (result.IsError)
            return Problem(result.FirstError.Description);

        return NoContent();
    }

    [HttpPut("changestatus")]
    public async Task<IActionResult> ChangeStatus([FromBody] ShipmentDocumentUpdateDto shipment, CancellationToken ct = default)
    {
        var result = await _shipmentDocumentService.ChangeStatusAsync(shipment, ct);

        if (result.IsError && result.FirstError.Type == ErrorType.Validation)
            return BadRequest(result.FirstError.Description);

        if (result.IsError && result.FirstError.Type == ErrorType.Conflict)
            return Conflict(result.FirstError.Description);

        if (result.IsError)
            return Problem(result.FirstError.Description);

        return NoContent();
    }

    [HttpPost("filter")]
    public async Task<ActionResult<List<ShipmentDocumentDto>>> Filter([FromBody] FilterDto filter, CancellationToken ct = default)
    {
        var documents = await _shipmentDocumentService.FilterAsync(filter, ct);
        return Ok(documents);
    }
}