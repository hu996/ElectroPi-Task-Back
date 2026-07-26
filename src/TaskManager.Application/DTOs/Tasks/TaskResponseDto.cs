using TaskManager.Domain.Entities;

namespace TaskManager.Application.DTOs.Tasks;

public class TaskResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; }
    public DateTime? DueDate { get; set; }
    public int ProjectId { get; set; }
}



