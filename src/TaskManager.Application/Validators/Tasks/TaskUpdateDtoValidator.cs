using FluentValidation;
using TaskManager.Application.DTOs.Tasks;

namespace TaskManager.Application.Validators.Tasks;

public class TaskUpdateDtoValidator : AbstractValidator<TaskUpdateDto>
{
    public TaskUpdateDtoValidator()
    {
        RuleFor(task => task.Title)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(task => task.Description)
            .MaximumLength(1000);

        RuleFor(task => task.Status)
            .IsInEnum();

        RuleFor(task => task.ProjectId)
            .GreaterThan(0);

        RuleFor(task => task.DueDate)
            .Must(dueDate => !dueDate.HasValue || dueDate.Value > DateTime.Now)
            .WithMessage("Due date must be greater than the current date and time.");
    }
}
