using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Damoor.Infrastructure.Extensions;

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
        return services;
    }
}