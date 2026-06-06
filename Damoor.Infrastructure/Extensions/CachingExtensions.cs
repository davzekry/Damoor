using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Damoor.Infrastructure.Interfaces;
using Damoor.Infrastructure.Services;


namespace Damoor.Infrastructure.Extensions;

internal static class CachingExtensions
{
    internal static IServiceCollection AddCaching(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = config.GetConnectionString("Redis");
            options.InstanceName = "Damoor:";
        });

        services.AddScoped<ICacheService, RedisCacheService>();

        return services;
    }
}