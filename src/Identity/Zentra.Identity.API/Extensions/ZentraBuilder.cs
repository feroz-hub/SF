using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Zentra.ProxyService.Hosting;

namespace Zentra.Hosting.Extensions;

public static class ZentraBuilder
{
    private const string ObservabilityPipelineRegistrationKey = "zentra:observability-pipeline-registered";

    /// <summary>
    /// Registers Zentra security response headers (HSTS, X-Frame-Options, etc.)
    /// This MUST be registered before UseZentraEndpoint and UseZentraApi so that
    /// every OAuth/OIDC and management API response carries the required headers.
    /// </summary>
    public static IApplicationBuilder UseZentraSecurityHeaders(this IApplicationBuilder app)
    {
        app.UseMiddleware<SecurityHeadersMiddleware>();
        return app;
    }

    public static IApplicationBuilder UseZentraCorrelationId(this IApplicationBuilder app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        return app;
    }

    public static IApplicationBuilder UseZentraRequestObservability(this IApplicationBuilder app)
    {
        app.UseMiddleware<RequestObservabilityMiddleware>();
        return app;
    }

    public static IApplicationBuilder UseZentraEndpoint(this IApplicationBuilder app)
    {
        EnsureObservabilityPipeline(app);
        app.UseMiddleware<ZentraEndpointMiddleware>();
        return app;
    }

    public static IApplicationBuilder UseZentraApi(this IApplicationBuilder app)
    {
        EnsureObservabilityPipeline(app);
        app.UseMiddleware<ZentraApiMiddleware>();
        return app;
    }

    public static IEndpointRouteBuilder MapZentraHealthChecks(
        this IEndpointRouteBuilder endpoints,
        string liveRoute = "/health/live",
        string readyRoute = "/health/ready")
    {
        endpoints.MapHealthChecks(liveRoute, new HealthCheckOptions
        {
            Predicate = registration => registration.Tags.Contains("live")
        });

        endpoints.MapHealthChecks(readyRoute, new HealthCheckOptions
        {
            Predicate = registration => registration.Tags.Contains("ready"),
            ResponseWriter = WriteHealthResponseAsync
        });

        return endpoints;
    }

    private static void EnsureObservabilityPipeline(IApplicationBuilder app)
    {
        if (app.Properties.ContainsKey(ObservabilityPipelineRegistrationKey)) return;

        app.UseZentraCorrelationId();
        app.UseZentraRequestObservability();
        app.Properties[ObservabilityPipelineRegistrationKey] = true;
    }

    private static Task WriteHealthResponseAsync(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var payload = new
        {
            status = report.Status.ToString(),
            totalDurationMs = Math.Round(report.TotalDuration.TotalMilliseconds, 2),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                durationMs = Math.Round(entry.Value.Duration.TotalMilliseconds, 2)
            })
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}
