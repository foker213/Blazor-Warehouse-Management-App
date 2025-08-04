using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Domain.Repositories;

namespace WarehouseManagement.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class ResourcesController : ControllerBase
{
    private readonly IResourceRepository _repository;

    public ResourcesController(IResourceRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<T>>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<T>>>> GetAll()
    {
        try
        {
            var items = await _repository.GetAllAsync();
            return Ok(new ApiResponse<List<T>>(items));
        }
        catch (Exception ex)
        {
            return Problem(
                title: "Ошибка при получении данных",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<T>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<T>>> GetById(int id)
    {
        if (id <= 0)
            return BadRequest(new ApiResponse<T>("Некорректный ID"));

        var item = await _repository.GetByIdAsync(id);

        return item == null
            ? NotFound(new ApiResponse<T>("Ресурс не найден"))
            : Ok(new ApiResponse<T>(item));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<int>>> Create([FromBody] T entity)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse<T>(ModelState));

        var createdId = await _repository.CreateAsync(entity);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdId },
            new ApiResponse<int>(createdId));
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] T entity)
    {
        try
        {
            var success = await _repository.UpdateAsync(entity);
            return success ? NoContent() : NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<T>(ex.Message));
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _repository.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }

    [HttpPatch("{id:int}/state")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeState(int id)
    {
        var success = await _repository.ChangeStateAsync(id);
        return success ? NoContent() : NotFound();
    }
}
