using Microsoft.Extensions.DependencyInjection;
using TaskModule.Repositories;
using TaskModule.Services;

namespace TaskModule;

public static class DependencyInjection
{
    public static IServiceCollection AddTaskModule(this IServiceCollection services)
    {
        // Register repositories
        services.AddScoped<ITaskRepository, TaskRepository>();

        // Register services
        services.AddScoped<ITaskService, TaskService>();

        // Register AutoMapper
        services.AddAutoMapper(cfg => { }, typeof(DependencyInjection).Assembly);

        return services;
    }
}
