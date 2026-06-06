using Damoor.API.Filters;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;

namespace Damoor.API.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(static c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Damoor API",
                Version = "v1",
                Description = "Clean Architecture · CQRS · .NET 9"
            });

            c.OperationFilter<HttpMethodOperationFilter>();

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Enter: Bearer {token}",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            // ── OpenApiReference lives in Microsoft.OpenApi.Models ───
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {{
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference   // ← now resolves
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id   = "Bearer"
                    }
                }, []
            }});
        });

        return services;
    }
}