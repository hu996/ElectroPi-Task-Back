using TaskManager.Application.DTOs.Tasks;

namespace TaskManager.Application.DTOs.Projects;

public class ProjectResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<TaskResponseDto> Tasks { get; set; } = [];
}


