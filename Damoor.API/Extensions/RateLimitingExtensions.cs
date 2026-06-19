using System.Threading.RateLimiting;
using Damoor.Application.Common.Models;
using Microsoft.AspNetCore.RateLimiting;

namespace Damoor.API.Extensions;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddRateLimiting(
        this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode =
                StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (context, cancellationToken) =>
            {
                await context.HttpContext.Response.WriteAsJsonAsync(
                    ApiResponse<object>.Fail(
                        "Too many requests. Please try again later."),
                    cancellationToken);
            };

            options.AddFixedWindowLimiter("fixed", limiter =>
            {
                limiter.Window = TimeSpan.FromSeconds(10);
                limiter.PermitLimit = 10;
                limiter.QueueLimit = 2;
                limiter.QueueProcessingOrder =
                    QueueProcessingOrder.OldestFirst;
            });

            options.AddPolicy("strict", httpContext =>
                RateLimitPartition.GetSlidingWindowLimiter(
                    partitionKey:
                        httpContext.Connection.RemoteIpAddress?.ToString()
                        ?? "unknown",
                    factory: _ => new SlidingWindowRateLimiterOptions
                    {
                        Window = TimeSpan.FromMinutes(1),
                        SegmentsPerWindow = 3,
                        PermitLimit = 5,
                        QueueLimit = 0,
                        QueueProcessingOrder =
                            QueueProcessingOrder.OldestFirst
                    }));
        });

        return services;
    }
}
