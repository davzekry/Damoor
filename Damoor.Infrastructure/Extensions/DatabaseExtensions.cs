using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Damoor.Infrastructure.Persistence;
using Damoor.Infrastructure.Persistence.Interceptors; // Add this using statement

namespace Damoor.Infrastructure.Extensions;

internal static class DatabaseExtensions
{
    internal static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddScoped<AuditableEntityInterceptor>(); // Register the interceptor

        services.AddDbContext<DamoorDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<AuditableEntityInterceptor>();
            options.UseSqlServer(
                config.GetConnectionString("DefaultConnection"),
                sql =>
                {
                    sql.EnableRetryOnFailure(3);
                    sql.CommandTimeout(30);
                    sql.MigrationsAssembly(
                        typeof(DamoorDbContext).Assembly.FullName);
                })
                .AddInterceptors(interceptor); // Add the interceptor to DbContext
        });

        return services;
    }
}