using FluentValidation;
using TaskManager.Application.DTOs.Tasks;

namespace TaskManager.Application.Validators.Tasks;

public class TaskStatusUpdateDtoValidator : AbstractValidator<TaskStatusUpdateDto>
{
    public TaskStatusUpdateDtoValidator()
    {
        RuleFor(task => task.Status)
            .IsInEnum();
    }
}
