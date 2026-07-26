using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Services;

namespace TaskManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<Interfaces.Services.IProjectService, ProjectService>();
        services.AddScoped<Interfaces.Services.ITaskService, TaskService>();
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
