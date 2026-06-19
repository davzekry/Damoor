namespace Damoor.API.Extensions;

public static class ConfigurationValidationExtensions
{
    public static IConfiguration ValidateRequiredSecrets(
        this IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "ConnectionStrings:DefaultConnection is required. " +
                "Configure it with .NET User Secrets for local development " +
                "or the deployment environment's secret store.");
        }

        var jwtSecret = configuration["JwtSettings:SecretKey"];

        if (string.IsNullOrWhiteSpace(jwtSecret))
        {
            throw new InvalidOperationException(
                "JwtSettings:SecretKey is required. Configure it with .NET " +
                "User Secrets for local development or the deployment " +
                "environment's secret store.");
        }

        if (jwtSecret.Length < 32)
        {
            throw new InvalidOperationException(
                "JwtSettings:SecretKey must be at least 32 characters.");
        }

        return configuration;
    }
}
