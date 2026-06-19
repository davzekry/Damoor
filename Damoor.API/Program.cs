using Damoor.API.Extensions;
using Damoor.API.Filters;
using Damoor.API.Middleware;
using Damoor.Application;
using Damoor.Application.Common.Models;
using Damoor.Infrastructure;
using Damoor.Infrastructure.Identity;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Damoor.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //builder.AddSerilog();

            builder.Configuration.ValidateRequiredSecrets();
            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddSwagger();
            
            // advanced services
            builder.Services.AddApiVersioningConfig();
            builder.Services.AddRateLimiting();

            // advanced filter for Double-Click Protection
            builder.Services.AddScoped<IdempotencyFilter>();

            builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var errors = context.ModelState.Values
                            .SelectMany(value => value.Errors)
                            .Select(error =>
                                string.IsNullOrWhiteSpace(error.ErrorMessage)
                                    ? "The request body is invalid."
                                    : error.ErrorMessage);

                        return new BadRequestObjectResult(
                            ApiResponse<object>.Fail(
                                "Validation failed.",
                                errors));
                    };
                });
            builder.Services.AddEndpointsApiExplorer();


            var app = builder.Build();

            await app.Services.InitializeIdentityAsync(builder.Configuration);

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    // If you have multiple API versions, configure Swagger UI to show them
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Damoor API v1");
                });
            }

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            //app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();

            // Enable static files for serving images from wwwroot/uploads
            app.UseStaticFiles();

            // 1. Routing must come before Rate Limiting and Auth
            app.UseRouting();

            // 2. Rate Limiting must come after Routing but before Auth/Endpoints
            app.UseRateLimiter();

            // 3. Authentication & Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // 4. Map Controllers
            app.MapControllers();

            // 5. Map Health Checks endpoint
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.Run();
        }
    }
}
