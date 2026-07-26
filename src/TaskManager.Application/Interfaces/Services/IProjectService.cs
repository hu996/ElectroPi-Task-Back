using TaskManager.Application.DTOs.Projects;
using TaskManager.Application.DTOs.Tasks;

namespace TaskManager.Application.Interfaces.Services;

public interface IProjectService
{
    Task<List<ProjectResponseDto>> GetAllAsync();
    Task<ProjectResponseDto?> GetByIdAsync(int id);
    Task<ProjectResponseDto?> GetDetailsAsync(int id);
    Task<List<TaskResponseDto>?> GetProjectTasksAsync(int projectId);
    Task<ProjectResponseDto> CreateAsync(ProjectCreateDto dto);
    Task<bool> UpdateAsync(int id, ProjectUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}


