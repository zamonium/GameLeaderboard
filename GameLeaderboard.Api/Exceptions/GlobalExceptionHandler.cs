using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GameLeaderboard.Api.Exceptions;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> logger;
    private readonly IHostEnvironment env;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment env)
    {
        this.logger = logger;
        this.env = env;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(
            exception,
            "Unhandled exception on {Method} {Path}: {Message}",
            httpContext.Request.Method,
            httpContext.Request.Path,
            exception.Message);

        var (status, title) = exception switch
        {
            ArgumentException  => (StatusCodes.Status400BadRequest,  "Bad request"),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            KeyNotFoundException => (StatusCodes.Status404NotFound,  "Resource not found"),
            NotImplementedException => (StatusCodes.Status501NotImplemented, "Not implemented"),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
        };

        var problemDetails = new ProblemDetails
        {
            Status = status,
            Title  = title,
            Detail = env.IsDevelopment() ? exception.Message : title
        };

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
