using Damoor.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Damoor.Infrastructure.Identity;

public static class IdentityInitializer
{
    public static async Task InitializeIdentityAsync(
        this IServiceProvider services,
        IConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

        foreach (var roleName in RoleNames.All)
        {
            if (await roleManager.RoleExistsAsync(roleName))
                continue;

            var result = await roleManager.CreateAsync(new IdentityRole<int>(roleName));
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Unable to create the '{roleName}' role: " +
                    string.Join("; ", result.Errors.Select(x => x.Description)));
            }
        }

        var settings = configuration.GetSection(AdminSeedSettings.SectionName)
            .Get<AdminSeedSettings>() ?? new AdminSeedSettings();

        if (!settings.Enabled)
            return;

        if (string.IsNullOrWhiteSpace(settings.FullName) ||
            string.IsNullOrWhiteSpace(settings.Email) ||
            string.IsNullOrWhiteSpace(settings.Password))
        {
            throw new InvalidOperationException(
                "AdminSeed FullName, Email, and Password are required when AdminSeed is enabled.");
        }

        var email = settings.Email.Trim();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<DamoorDbContext>();
        var existingUser = await userManager.FindByEmailAsync(email);

        if (existingUser is not null)
        {
            if (!await userManager.IsInRoleAsync(existingUser, RoleNames.Admin))
            {
                throw new InvalidOperationException(
                    "The configured Admin email belongs to a non-Admin account.");
            }

            return;
        }

        var executionStrategy =
            dbContext.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction =
                await dbContext.Database.BeginTransactionAsync(
                    cancellationToken);

            var admin = new AppUser
            {
                FullName = settings.FullName.Trim(),
                Email = email,
                UserName = email,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(
                admin,
                settings.Password);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Unable to create the configured Admin account: " +
                    string.Join(
                        "; ",
                        createResult.Errors.Select(x => x.Description)));
            }

            var roleResult = await userManager.AddToRoleAsync(
                admin,
                RoleNames.Admin);
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Unable to assign the Admin role: " +
                    string.Join(
                        "; ",
                        roleResult.Errors.Select(x => x.Description)));
            }

            await transaction.CommitAsync(cancellationToken);
        });
    }
}
