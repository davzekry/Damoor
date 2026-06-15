using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Damoor.Infrastructure.Extensions;
using Damoor.Infrastructure.Persistence;
using Damoor.Infrastructure.Services;
using Damoor.Infrastructure.Interfaces;

namespace Damoor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddIdentityServices();
        services.AddCaching(configuration);

        // Register LocalFileService and its interface
        services.AddScoped<IFileService, LocalFileService>();

        // Add Health Checks
        services.AddHealthChecks()
            .AddDbContextCheck<DamoorDbContext>()
            .AddRedis(configuration.GetConnectionString("Redis")!);

        return services;
    }
}