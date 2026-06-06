using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Damoor.Infrastructure.Persistence;

namespace Damoor.Infrastructure.Extensions;

internal static class DatabaseExtensions
{
    internal static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddDbContext<PrStoreDbContext>(options =>
            options.UseSqlServer(
                config.GetConnectionString("DefaultConnection"),
                sql =>
                {
                    sql.EnableRetryOnFailure(3);
                    sql.CommandTimeout(30);
                    sql.MigrationsAssembly(
                        typeof(PrStoreDbContext).Assembly.FullName);
                }));

        return services;
    }
}