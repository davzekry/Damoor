using Damoor.Application.Common.Exceptions;
using Damoor.Application.Common.Models;
using FluentValidation;

namespace Damoor.API.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException exception)
        {
            _logger.LogWarning(
                "Request validation failed with {ErrorCount} error(s).",
                exception.Errors.Count());

            await WriteErrorAsync(
                context,
                StatusCodes.Status400BadRequest,
                "Validation failed.",
                exception.Errors.Select(x => x.ErrorMessage));
        }
        catch (BadRequestException exception)
        {
            await WriteErrorAsync(
                context,
                StatusCodes.Status400BadRequest,
                exception.Message,
                exception.Errors);
        }
        catch (UnauthorizedException exception)
        {
            await WriteErrorAsync(
                context,
                StatusCodes.Status401Unauthorized,
                exception.Message);
        }
        catch (ConflictException exception)
        {
            await WriteErrorAsync(
                context,
                StatusCodes.Status409Conflict,
                exception.Message);
        }
        catch (NotFoundException exception)
        {
            await WriteErrorAsync(
                context,
                StatusCodes.Status404NotFound,
                exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception");
            await WriteErrorAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.");
        }
    }

    private static async Task WriteErrorAsync(
        HttpContext context,
        int statusCode,
        string message,
        IEnumerable<string>? errors = null)
    {
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(
            ApiResponse<object>.Fail(message, errors));
    }
}
