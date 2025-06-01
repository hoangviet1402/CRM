using Menu.Repositories;
using Menu.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Menu;

public static class DependencyInjection
{
    public static IServiceCollection AddMenuModule(this IServiceCollection services)
    {
        // Register repositories
        services.AddScoped<IMenuRepository, MenuRepository>();

        // Register services
        services.AddScoped<IMenuService, MenuService>();

        // Register AutoMapper
        services.AddAutoMapper(cfg => { }, typeof(DependencyInjection).Assembly);

        return services;
    }
}
