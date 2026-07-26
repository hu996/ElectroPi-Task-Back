using TaskManager.Application.DTOs.Projects;
using TaskManager.Application.DTOs.Tasks;
using TaskManager.Domain.Entities;
using TaskManager.Application.Exceptions;
using TaskManager.Application.Interfaces.Repositories;
using TaskManager.Application.Interfaces.Services;

namespace TaskManager.Application.Services;

public class ProjectService(IProjectRepository projectRepository, ITaskRepository taskRepository) : IProjectService
{
    public async Task<List<ProjectResponseDto>> GetAllAsync()
    {
        var projects = await projectRepository.GetAllAsync();
        return projects.Select(MapProject).ToList();
    }

    public async Task<ProjectResponseDto?> GetByIdAsync(int id)
    {
        var project = await projectRepository.GetByIdAsync(id);
        return project is null ? null : MapProject(project);
    }

    public async Task<ProjectResponseDto?> GetDetailsAsync(int id)
    {
        var project = await projectRepository.GetWithTasksAsync(id);
        return project is null ? null : MapProject(project);
    }

    public async Task<List<TaskResponseDto>?> GetProjectTasksAsync(int projectId)
    {
        if (!await projectRepository.ExistsAsync(projectId))
        {
            return null;
        }

        var tasks = await taskRepository.GetByProjectIdAsync(projectId);
        return tasks.Select(MapTask).ToList();
    }

    public async Task<ProjectResponseDto> CreateAsync(ProjectCreateDto dto)
    {
        var name = dto.Name.Trim();
        if (await projectRepository.NameExistsAsync(name))
        {
            throw new DuplicateResourceException($"Project '{name}' already exists.");
        }

        var project = new Project
        {
            Name = name,
            Description = dto.Description?.Trim()
        };

        var createdProject = await projectRepository.CreateAsync(project);
        return MapProject(createdProject);
    }

    public async Task<bool> UpdateAsync(int id, ProjectUpdateDto dto)
    {
        var project = await projectRepository.GetByIdAsync(id);
        if (project is null)
        {
            return false;
        }

        var name = dto.Name.Trim();
        if (await projectRepository.NameExistsAsync(name, id))
        {
            throw new DuplicateResourceException($"Project '{name}' already exists.");
        }

        project.Name = name;
        project.Description = dto.Description?.Trim();

        await projectRepository.UpdateAsync(project);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var project = await projectRepository.GetByIdAsync(id);
        if (project is null)
        {
            return false;
        }

        await projectRepository.DeleteAsync(project);
        return true;
    }

    private static ProjectResponseDto MapProject(Project project)
    {
        return new ProjectResponseDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            CreatedAt = project.CreatedAt,
            Tasks = project.Tasks.Select(MapTask).ToList()
        };
    }

    private static TaskResponseDto MapTask(TaskItem task)
    {
        return new TaskResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            DueDate = task.DueDate,
            ProjectId = task.ProjectId
        };
    }
}
