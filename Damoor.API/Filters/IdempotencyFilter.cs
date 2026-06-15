using Damoor.Infrastructure.Interfaces; // ICacheService is in Infrastructure
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace Damoor.API.Filters;

public class IdempotencyFilter : IAsyncActionFilter
{
    private readonly ICacheService _cache;

    public IdempotencyFilter(ICacheService cache)
    {
        _cache = cache;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.HttpContext.Request.Method != HttpMethods.Post &&
            context.HttpContext.Request.Method != HttpMethods.Put) // Also apply to PUT for idempotency
        {
            await next();
            return;
        }

        if (!context.HttpContext.Request.Headers.TryGetValue("X-Idempotency-Key", out var key) || string.IsNullOrEmpty(key))
        {
            context.Result = new BadRequestObjectResult("X-Idempotency-Key header is missing or empty.");
            return;
        }

        var idempotencyKey = key.ToString();
        var cachedResult = await _cache.GetAsync<string>(idempotencyKey);
        if (cachedResult != null)
        {
            context.Result = new ContentResult
            {
                Content = cachedResult,
                ContentType = "application/json",
                StatusCode = StatusCodes.Status200OK // Return 200 OK for cached idempotent requests
            };
            return;
        }

        var executedContext = await next();

        // Cache the result only if the action was successful (e.g., 2xx status code)
        if (executedContext.Result is ObjectResult objectResult && objectResult.StatusCode >= 200 && objectResult.StatusCode < 300)
        {
            await _cache.SetAsync(idempotencyKey, JsonSerializer.Serialize(objectResult.Value), TimeSpan.FromHours(24));
        }
    }
}