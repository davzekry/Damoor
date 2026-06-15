using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
namespace Damoor.API.Extensions;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Global policy for general endpoints
            options.AddFixedWindowLimiter("fixed", opt =>
            {
                opt.Window = TimeSpan.FromSeconds(10);
                opt.PermitLimit = 10; // 10 requests every 10 seconds
                opt.QueueLimit = 2;
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });

            // Stricter policy for Auth/Checkout
            options.AddSlidingWindowLimiter("strict", opt =>
            {
                opt.Window = TimeSpan.FromMinutes(1);
                opt.SegmentsPerWindow = 3;
                opt.PermitLimit = 5; // Only 5 attempts per minute
            });
        });

        return services;
    }
}