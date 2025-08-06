using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Application.IServices;
using WarehouseManagement.Contracts;
using WarehouseManagement.Contracts.Receipt;

namespace WarehouseManagement.Server.Controllers;

[ApiController]
[Route(Routes.Receipts.Api)]
public class ReceiptDocumentsController : ControllerBase
{
    private readonly IReceiptDocumentService _receiptDocumentService;

    public ReceiptDocumentsController(IReceiptDocumentService receiptDocumentService)
    {
        _receiptDocumentService = receiptDocumentService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ReceiptDocumentDto>>> GetAll(CancellationToken ct = default)
    {
        var documents = await _receiptDocumentService.GetAll(ct);
        return Ok(documents);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReceiptDocumentDto>> GetById(int id, CancellationToken ct = default)
    {
        var result = await _receiptDocumentService.GetBy(id, ct);

        if (result.IsError && result.FirstError.Type == ErrorType.NotFound)
            return NotFound();

        if (result.IsError)
            return Problem(result.FirstError.Description);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ReceiptDocumentDto receipt, CancellationToken ct = default)
    {
        var result = await _receiptDocumentService.CreateAsync(receipt, ct);

        if (result.IsError && result.FirstError.Type == ErrorType.Validation)
            return BadRequest(result.FirstError.Description);

        if (result.IsError && result.FirstError.Type == ErrorType.Conflict)
            return Conflict(result.FirstError.Description);

        if (result.IsError)
            return Problem(result.FirstError.Description);

        return CreatedAtAction(nameof(GetById), new { id = receipt.Id }, receipt);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] ReceiptDocumentDto receipt, CancellationToken ct = default)
    {
        var result = await _receiptDocumentService.UpdateAsync(receipt, ct);

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
        var result = await _receiptDocumentService.DeleteAsync(id, ct);

        if (result.IsError && result.FirstError.Type == ErrorType.NotFound)
            return NotFound();

        if (result.IsError)
            return Problem(result.FirstError.Description);

        return NoContent();
    }

    [HttpPost("filter")]
    public async Task<ActionResult<List<ReceiptDocumentDto>>> Filter([FromBody] FilterDto filter, CancellationToken ct = default)
    {
        var documents = await _receiptDocumentService.FilterAsync(filter, ct);
        return Ok(documents);
    }
}