using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs.Projects;
using TaskManager.Application.DTOs.Tasks;
using TaskManager.Application.Interfaces.Services;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController(IProjectService projectService, ITaskService taskService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ProjectResponseDto>>> GetAll()
    {
        return Ok(await projectService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectResponseDto>> GetById(int id)
    {
        var project = await projectService.GetByIdAsync(id);
        return project is null ? NotFound() : Ok(project);
    }

    [HttpGet("{id:int}/details")]
    public async Task<ActionResult<ProjectResponseDto>> GetDetails(int id)
    {
        var project = await projectService.GetDetailsAsync(id);
        return project is null ? NotFound() : Ok(project);
    }

    [HttpGet("{id:int}/tasks")]
    public async Task<IActionResult> GetProjectTasks(int id)
    {
        var tasks = await projectService.GetProjectTasksAsync(id);
        return tasks is null ? NotFound() : Ok(tasks);
    }

    [HttpPost("{id:int}/tasks")]
    public async Task<ActionResult<TaskResponseDto>> CreateTask(int id, ProjectTaskCreateDto dto)
    {
        var task = await taskService.CreateForProjectAsync(id, dto);
        if (task is null)
        {
            return NotFound(new { message = "Project does not exist." });
        }

        return CreatedAtAction(nameof(GetProjectTasks), new { id }, task);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectResponseDto>> Create(ProjectCreateDto dto)
    {
        var project = await projectService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ProjectUpdateDto dto)
    {
        var updated = await projectService.UpdateAsync(id, dto);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await projectService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}

