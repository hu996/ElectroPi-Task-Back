using FluentValidation;
using TaskManager.Application.DTOs.Projects;

namespace TaskManager.Application.Validators.Projects;

public class ProjectCreateDtoValidator : AbstractValidator<ProjectCreateDto>
{
    public ProjectCreateDtoValidator()
    {
        RuleFor(project => project.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(project => project.Description)
            .MaximumLength(500);
    }
}
