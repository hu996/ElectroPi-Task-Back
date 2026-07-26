using Microsoft.EntityFrameworkCore;
using TaskManager.Infrastructure.Persistence;
using TaskManager.Domain.Entities;
using TaskManager.Application.Interfaces.Repositories;

namespace TaskManager.Infrastructure.Repositories;

public class ProjectRepository(AppDbContext context) : IProjectRepository
{
    public Task<List<Project>> GetAllAsync()
    {
        return context.Projects
            .AsNoTracking()
            .Include(project => project.Tasks.OrderBy(task => task.DueDate))
            .OrderByDescending(project => project.CreatedAt)
            .ToListAsync();
    }

    public Task<Project?> GetByIdAsync(int id)
    {
        return context.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(project => project.Id == id);
    }

    public Task<Project?> GetWithTasksAsync(int id)
    {
        return context.Projects
            .AsNoTracking()
            .Include(project => project.Tasks.OrderBy(task => task.DueDate))
            .FirstOrDefaultAsync(project => project.Id == id);
    }

    public async Task<Project> CreateAsync(Project project)
    {
        context.Projects.Add(project);
        await context.SaveChangesAsync();
        return project;
    }

    public async Task UpdateAsync(Project project)
    {
        context.Projects.Update(project);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Project project)
    {
        context.Projects.Remove(project);
        await context.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(int id)
    {
        return context.Projects.AnyAsync(project => project.Id == id);
    }

    public Task<bool> NameExistsAsync(string name, int? excludedProjectId = null)
    {
        var normalizedName = name.Trim().ToUpper();

        return context.Projects.AnyAsync(project =>
            project.Name.ToUpper() == normalizedName &&
            (!excludedProjectId.HasValue || project.Id != excludedProjectId.Value));
    }
}

