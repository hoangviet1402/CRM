using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace TaskModule;

public static class TaskModuleStartup
{
    public static void AddTaskModuleServices(this IServiceCollection services)
    {
        services.AddTaskModule();
    }

    public static void UseTaskModule(this IApplicationBuilder app)
    {
        // Add middleware if needed
    }
}
