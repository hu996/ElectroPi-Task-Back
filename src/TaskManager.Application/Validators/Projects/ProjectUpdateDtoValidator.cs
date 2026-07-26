using FluentValidation;
using TaskManager.Application.DTOs.Projects;

namespace TaskManager.Application.Validators.Projects;

public class ProjectUpdateDtoValidator : AbstractValidator<ProjectUpdateDto>
{
    public ProjectUpdateDtoValidator()
    {
        RuleFor(project => project.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(project => project.Description)
            .MaximumLength(500);
    }
}
