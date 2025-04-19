using Education.Repositories;
using Education.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Education;

public static class DependencyInjection
{
    public static IServiceCollection AddEducationModule(this IServiceCollection services)
    {
        // Register repositories
        services.AddScoped<IEducationRepository, EducationRepository>();

        // Register services
        services.AddScoped<IEducationService, EducationService>();

        // Register AutoMapper
        services.AddAutoMapper(cfg => { }, typeof(DependencyInjection).Assembly);

        return services;
    }
} 