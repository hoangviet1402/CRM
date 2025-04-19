using EmployeeModule.Repositories;
using EmployeeModule.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeModule;

public static class DependencyInjection
{
    public static IServiceCollection AddEmployeeModule(this IServiceCollection services)
    {
        // Register repositories
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();

        // Register services
        services.AddScoped<IEmployeeService, EmployeeService>();

        // Register AutoMapper
        services.AddAutoMapper(cfg => { }, typeof(DependencyInjection).Assembly);

        return services;
    }
} 