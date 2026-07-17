using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Zentra.Domain;
using Zentra.Domain.Constants;
using Zentra.DomainServices.Infra;

namespace Zentra.ProxyService.Hosting;

public class RequestObservabilityMiddleware
{
    private readonly ILogger<RequestObservabilityMiddleware> applicationLogger;
    private readonly ILoggerService loggerService;
    private readonly bool metricsEnabled;
    private readonly RequestDelegate next;

    public RequestObservabilityMiddleware(
        RequestDelegate next,
        ILogger<RequestObservabilityMiddleware> applicationLogger,
        ILoggerInstance loggerInstance,
        IConfiguration configuration)
    {
        this.next = next;
        this.applicationLogger = applicationLogger;

        loggerService = loggerInstance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
        metricsEnabled = configuration.GetValue("Zentra:Observability:EnableMetrics", false);
    }

    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
    {
        var startTimestamp = Stopwatch.GetTimestamp();

        try
        {
            await next(context);
        }
        finally
        {
            var elapsed = Stopwatch.GetElapsedTime(startTimestamp);
            var latencyMs = elapsed.TotalMilliseconds;

            var correlationId = ResolveCorrelationId(context);
            var tenantId = LogRedactionHelper.GetSafeTenantId(tenantContext.TenantId);
            var userId = LogRedactionHelper.GetSafeUserId(ResolveUserId(context.User));
            var route = context.Request.Path.HasValue ? context.Request.Path.Value : "/";
            var statusCode = context.Response.StatusCode;

            WriteStructuredRequestLog(correlationId, tenantId, userId, route, statusCode, latencyMs);

            if (metricsEnabled)
            {
                var routeGroup = NormalizeRouteGroup(route);
                ZentraMetrics.RecordRequest(context.Request.Method, routeGroup, statusCode, latencyMs);
            }
        }
    }

    private void WriteStructuredRequestLog(
        string correlationId,
        string tenantId,
        string userId,
        string? route,
        int statusCode,
        double latencyMs)
    {
        const string template =
            "request_completed correlationId={CorrelationId} tenantId={TenantId} userId={UserId} route={Route} statusCode={StatusCode} latencyMs={LatencyMs}";

        var normalizedRoute = string.IsNullOrWhiteSpace(route) ? "/" : route;
        var roundedLatencyMs = Math.Round(latencyMs, 2);

        if (loggerService != null)
        {
            loggerService.WriteTo(
                Log.Information,
                template,
                correlationId,
                tenantId,
                userId,
                normalizedRoute,
                statusCode,
                roundedLatencyMs);
            return;
        }

        applicationLogger.LogInformation(
            template,
            correlationId,
            tenantId,
            userId,
            normalizedRoute,
            statusCode,
            roundedLatencyMs);
    }

    private static string ResolveCorrelationId(HttpContext context)
    {
        if (context.Items.TryGetValue(ObservabilityConstants.CorrelationIdItemKey, out var correlationId) &&
            correlationId is string correlation)
            return correlation;

        if (!string.IsNullOrWhiteSpace(context.TraceIdentifier)) return context.TraceIdentifier;

        return Guid.NewGuid().ToString("N");
    }

    private static string? ResolveUserId(ClaimsPrincipal principal)
    {
        if (principal?.Identity?.IsAuthenticated != true) return null;

        return principal.FindFirst("sub")?.Value
               ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
               ?? principal.Identity?.Name;
    }

    private static string NormalizeRouteGroup(string? route)
    {
        if (string.IsNullOrWhiteSpace(route)) return "/";

        if (route.StartsWith(ApiRoutePathConstants.BasePath, StringComparison.OrdinalIgnoreCase))
            return "/security/api";

        if (route.StartsWith("/security/", StringComparison.OrdinalIgnoreCase))
        {
            var segments = route.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length >= 2) return $"/security/{segments[1].ToLowerInvariant()}";

            return "/security";
        }

        var rootSegment = route.Split('/', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        return string.IsNullOrWhiteSpace(rootSegment) ? "/" : $"/{rootSegment.ToLowerInvariant()}";
    }
}
