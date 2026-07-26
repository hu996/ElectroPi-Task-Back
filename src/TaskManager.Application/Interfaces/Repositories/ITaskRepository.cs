using TaskManager.Domain.Entities;

namespace TaskManager.Application.Interfaces.Repositories;

public interface ITaskRepository
{
    Task<List<TaskItem>> GetAllAsync();
    Task<TaskItem?> GetByIdAsync(int id);
    Task<List<TaskItem>> GetByProjectIdAsync(int projectId);
    Task<List<TaskItem>> GetByStatusAsync(TaskItemStatus status);
    Task<TaskItem> CreateAsync(TaskItem task);
    Task UpdateAsync(TaskItem task);
    Task DeleteAsync(TaskItem task);
    Task<bool> HasOpenTaskWithTitleAsync(int projectId, string title, int? excludedTaskId = null);
}
