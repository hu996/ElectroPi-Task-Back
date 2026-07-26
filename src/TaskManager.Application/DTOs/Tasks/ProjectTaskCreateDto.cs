using TaskManager.Domain.Entities;

namespace TaskManager.Application.DTOs.Tasks;

public class ProjectTaskCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; } = TaskItemStatus.ToDo;
    public DateTime? DueDate { get; set; }
}
