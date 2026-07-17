using System.Diagnostics;
using System.Text.Json;

namespace ZentraInstallerMVC.Middleware;

public sealed class GlobalExceptionMiddleware
{
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
        catch (Exception ex)
        {
            var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
            _logger.LogError(ex, "Unhandled installer exception. TraceId: {TraceId}", traceId);

            if (context.Response.HasStarted) throw;

            context.Response.Clear();

            if (IsHtmlRequest(context.Request))
            {
                context.Response.StatusCode = StatusCodes.Status302Found;
                context.Response.Headers.Location = "/error";
                return;
            }

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";

            var problem = new
            {
                title = "Internal Server Error",
                status = 500,
                traceId
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }

    private static bool IsHtmlRequest(HttpRequest request)
    {
        if (request.Headers.TryGetValue("Accept", out var acceptValues))
            return acceptValues.Any(value => value?.Contains("text/html", StringComparison.OrdinalIgnoreCase) == true);

        return false;
    }
}
