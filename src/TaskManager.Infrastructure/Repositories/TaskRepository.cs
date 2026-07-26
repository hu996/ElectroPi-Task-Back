using Microsoft.EntityFrameworkCore;
using TaskManager.Infrastructure.Persistence;
using TaskManager.Domain.Entities;
using TaskManager.Application.Interfaces.Repositories;

namespace TaskManager.Infrastructure.Repositories;

public class TaskRepository(AppDbContext context) : ITaskRepository
{
    public Task<List<TaskItem>> GetAllAsync()
    {
        return context.Tasks
            .AsNoTracking()
            .OrderBy(task => task.DueDate)
            .ToListAsync();
    }

    public Task<TaskItem?> GetByIdAsync(int id)
    {
        return context.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(task => task.Id == id);
    }

    public Task<List<TaskItem>> GetByProjectIdAsync(int projectId)
    {
        return context.Tasks
            .AsNoTracking()
            .Where(task => task.ProjectId == projectId)
            .OrderBy(task => task.DueDate)
            .ToListAsync();
    }

    public Task<List<TaskItem>> GetByStatusAsync(TaskItemStatus status)
    {
        return context.Tasks
            .AsNoTracking()
            .Where(task => task.Status == status)
            .OrderBy(task => task.DueDate)
            .ToListAsync();
    }

    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
        context.Tasks.Add(task);
        await context.SaveChangesAsync();
        return task;
    }

    public async Task UpdateAsync(TaskItem task)
    {
        context.Tasks.Update(task);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TaskItem task)
    {
        context.Tasks.Remove(task);
        await context.SaveChangesAsync();
    }

    public Task<bool> HasOpenTaskWithTitleAsync(int projectId, string title, int? excludedTaskId = null)
    {
        var normalizedTitle = title.Trim().ToUpper();

        return context.Tasks.AnyAsync(task =>
            task.ProjectId == projectId &&
            task.Title.ToUpper() == normalizedTitle &&
            task.Status != TaskItemStatus.Done &&
            (!excludedTaskId.HasValue || task.Id != excludedTaskId.Value));
    }
}
