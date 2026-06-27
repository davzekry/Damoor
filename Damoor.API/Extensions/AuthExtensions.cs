using System.Text;
using Damoor.API.Services;
using Damoor.Application.Common.Models;
using Damoor.Application.Features.Authentication.Common;
using Damoor.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Damoor.API.Extensions;

public static class AuthExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection(JwtSettings.SectionName);
        var jwt = jwtSection.Get<JwtSettings>()
            ?? throw new InvalidOperationException("JwtSettings are required.");

        services.AddOptions<JwtSettings>()
            .Bind(jwtSection)
            .Validate(
                settings =>
                    !string.IsNullOrWhiteSpace(settings.SecretKey) &&
                    settings.SecretKey.Length >= 32 &&
                    !string.IsNullOrWhiteSpace(settings.Issuer) &&
                    !string.IsNullOrWhiteSpace(settings.Audience) &&
                    settings.ExpiryMinutes > 0,
                "JwtSettings must contain a 32+ character SecretKey, " +
                "Issuer, Audience, and a positive ExpiryMinutes value.")
            .ValidateOnStart();
        services.AddScoped<IAccessTokenService, JwtAccessTokenService>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwt.Issuer,
                ValidAudience = jwt.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwt.SecretKey)),
                NameClaimType = "name",
                RoleClaimType = "role"
            };
            options.Events = new JwtBearerEvents
            {
                OnChallenge = async context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode =
                        StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(
                        ApiResponse<object>.Fail(
                            "Authentication is required."));
                },
                OnForbidden = async context =>
                {
                    context.Response.StatusCode =
                        StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(
                        ApiResponse<object>.Fail(
                            "You are not authorized to access this resource."));
                }
            };
        });

        services.AddAuthorizationBuilder()
            .AddPolicy(
                PolicyNames.AdminOnly,
                policy => policy.RequireRole(RoleNames.Admin));

        return services;
    }
}
