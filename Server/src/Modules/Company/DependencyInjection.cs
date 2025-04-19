using Company.Repositories;
using Company.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Company;

public static class DependencyInjection
{
    public static IServiceCollection AddCompanyModule(this IServiceCollection services)
    {
        // Register repositories
        services.AddScoped<ICompanyRepository, CompanyRepository>();

        // Register services
        services.AddScoped<ICompanyService, CompanyService>();

        // Register AutoMapper
        services.AddAutoMapper(cfg => { }, typeof(DependencyInjection).Assembly);

        return services;
    }
} 