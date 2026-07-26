using TaskManager.Application.DTOs.Tasks;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Interfaces.Services;

public interface ITaskService
{
    Task<List<TaskResponseDto>> GetAllAsync();
    Task<TaskResponseDto?> GetByIdAsync(int id);
    Task<List<TaskResponseDto>> GetByStatusAsync(TaskItemStatus status);
    Task<TaskResponseDto?> CreateAsync(TaskCreateDto dto);
    Task<TaskResponseDto?> CreateForProjectAsync(int projectId, ProjectTaskCreateDto dto);
    Task<bool> UpdateAsync(int id, TaskUpdateDto dto);
    Task<bool> UpdateStatusAsync(int id, TaskStatusUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}

