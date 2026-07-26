using TaskManager.Application.DTOs.Tasks;
using TaskManager.Domain.Entities;
using TaskManager.Application.Exceptions;
using TaskManager.Application.Interfaces.Repositories;
using TaskManager.Application.Interfaces.Services;

namespace TaskManager.Application.Services;

public class TaskService(ITaskRepository taskRepository, IProjectRepository projectRepository) : ITaskService
{
    public async Task<List<TaskResponseDto>> GetAllAsync()
    {
        var tasks = await taskRepository.GetAllAsync();
        return tasks.Select(MapTask).ToList();
    }

    public async Task<TaskResponseDto?> GetByIdAsync(int id)
    {
        var task = await taskRepository.GetByIdAsync(id);
        return task is null ? null : MapTask(task);
    }

    public async Task<List<TaskResponseDto>> GetByStatusAsync(TaskItemStatus status)
    {
        var tasks = await taskRepository.GetByStatusAsync(status);
        return tasks.Select(MapTask).ToList();
    }

    public async Task<TaskResponseDto?> CreateAsync(TaskCreateDto dto)
    {
        if (!await projectRepository.ExistsAsync(dto.ProjectId))
        {
            return null;
        }

        var title = dto.Title.Trim();
        if (dto.Status != TaskItemStatus.Done && await taskRepository.HasOpenTaskWithTitleAsync(dto.ProjectId, title))
        {
            throw new DuplicateResourceException($"Task '{title}' already exists as ToDo or InProgress in this project.");
        }

        var task = new TaskItem
        {
            Title = title,
            Description = dto.Description?.Trim(),
            Status = dto.Status,
            DueDate = dto.DueDate,
            ProjectId = dto.ProjectId
        };

        var createdTask = await taskRepository.CreateAsync(task);
        return MapTask(createdTask);
    }


    public Task<TaskResponseDto?> CreateForProjectAsync(int projectId, ProjectTaskCreateDto dto)
    {
        return CreateAsync(new TaskCreateDto
        {
            Title = dto.Title,
            Description = dto.Description,
            Status = dto.Status,
            DueDate = dto.DueDate,
            ProjectId = projectId
        });
    }
    public async Task<bool> UpdateAsync(int id, TaskUpdateDto dto)
    {
        var task = await taskRepository.GetByIdAsync(id);
        if (task is null || !await projectRepository.ExistsAsync(dto.ProjectId))
        {
            return false;
        }

        var title = dto.Title.Trim();
        if (dto.Status != TaskItemStatus.Done && await taskRepository.HasOpenTaskWithTitleAsync(dto.ProjectId, title, id))
        {
            throw new DuplicateResourceException($"Task '{title}' already exists as ToDo or InProgress in this project.");
        }

        task.Title = title;
        task.Description = dto.Description?.Trim();
        task.Status = dto.Status;
        task.DueDate = dto.DueDate;
        task.ProjectId = dto.ProjectId;

        await taskRepository.UpdateAsync(task);
        return true;
    }

    public async Task<bool> UpdateStatusAsync(int id, TaskStatusUpdateDto dto)
    {
        var task = await taskRepository.GetByIdAsync(id);
        if (task is null)
        {
            return false;
        }

        if (dto.Status != TaskItemStatus.Done && await taskRepository.HasOpenTaskWithTitleAsync(task.ProjectId, task.Title, id))
        {
            throw new DuplicateResourceException($"Task '{task.Title}' already exists as ToDo or InProgress in this project.");
        }

        task.Status = dto.Status;
        await taskRepository.UpdateAsync(task);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var task = await taskRepository.GetByIdAsync(id);
        if (task is null)
        {
            return false;
        }

        await taskRepository.DeleteAsync(task);
        return true;
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


