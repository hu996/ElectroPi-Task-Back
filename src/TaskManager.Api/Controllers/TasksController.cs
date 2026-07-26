using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs.Tasks;
using TaskManager.Domain.Entities;
using TaskManager.Application.Interfaces.Services;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController(ITaskService taskService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TaskResponseDto>>> GetAll()
    {
        return Ok(await taskService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskResponseDto>> GetById(int id)
    {
        var task = await taskService.GetByIdAsync(id);
        return task is null ? NotFound() : Ok(task);
    }

    [HttpGet("status/{status}")]
    public async Task<ActionResult<List<TaskResponseDto>>> GetByStatus(TaskItemStatus status)
    {
        return Ok(await taskService.GetByStatusAsync(status));
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponseDto>> Create(TaskCreateDto dto)
    {
        var task = await taskService.CreateAsync(dto);
        if (task is null)
        {
            return BadRequest(new { message = "ProjectId does not exist." });
        }

        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, TaskUpdateDto dto)
    {
        var updated = await taskService.UpdateAsync(id, dto);
        return updated ? NoContent() : NotFound();
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, TaskStatusUpdateDto dto)
    {
        var updated = await taskService.UpdateStatusAsync(id, dto);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await taskService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}

