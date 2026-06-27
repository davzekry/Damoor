// Infrastructure/Extensions/IdentityExtensions.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Damoor.Infrastructure.Identity;
using Damoor.Infrastructure.Persistence;

namespace Damoor.Infrastructure.Extensions;

internal static class IdentityExtensions
{
    internal static IServiceCollection AddIdentityServices(
        this IServiceCollection services)
    {
        services
            .AddIdentity<AppUser, IdentityRole<int>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 5;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
            .AddEntityFrameworkStores<DamoorDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}
