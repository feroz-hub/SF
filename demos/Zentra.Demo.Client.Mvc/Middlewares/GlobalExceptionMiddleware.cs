using System.Net;

namespace Zentra.DemoClientMvc.Middlewares;

public sealed class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unhandled exception for {Method} {Path}. TraceIdentifier: {TraceIdentifier}",
                context.Request.Method,
                context.Request.Path,
                context.TraceIdentifier);

            if (context.Response.HasStarted) throw;

            context.Response.Clear();

            if (IsApiRequest(context))
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsJsonAsync(new
                {
                    errorCode = "UnhandledError",
                    errorMessage = "An unexpected server error occurred.",
                    traceId = context.TraceIdentifier
                });
                return;
            }

            var encodedMessage = Uri.EscapeDataString(ex.Message);
            context.Response.Redirect($"/Error/Error?errorCode=UnhandledError&errorMessage={encodedMessage}");
        }
    }

    private static bool IsApiRequest(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        if (path.StartsWith("/api", StringComparison.OrdinalIgnoreCase)) return true;

        var accept = context.Request.Headers.Accept.ToString();
        return accept.Contains("application/json", StringComparison.OrdinalIgnoreCase);
    }
}
