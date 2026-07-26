using TaskManager.Domain.Entities;

namespace TaskManager.Application.Interfaces.Repositories;

public interface IProjectRepository
{
    Task<List<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(int id);
    Task<Project?> GetWithTasksAsync(int id);
    Task<Project> CreateAsync(Project project);
    Task UpdateAsync(Project project);
    Task DeleteAsync(Project project);
    Task<bool> ExistsAsync(int id);
    Task<bool> NameExistsAsync(string name, int? excludedProjectId = null);
}
