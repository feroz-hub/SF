using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using HCL.CS.ProxyService.Hosting;

namespace HCL.CS.Hosting.Extensions;

public static class HclCsBuilder
{
    private const string ObservabilityPipelineRegistrationKey = "hcl-cs:observability-pipeline-registered";

    /// <summary>
    /// Registers HCL.CS security response headers (HSTS, X-Frame-Options, etc.)
    /// This MUST be registered before UseHclCsEndpoint and UseHclCsApi so that
    /// every OAuth/OIDC and management API response carries the required headers.
    /// </summary>
    public static IApplicationBuilder UseHclCsSecurityHeaders(this IApplicationBuilder app)
    {
        app.UseMiddleware<SecurityHeadersMiddleware>();
        return app;
    }

    public static IApplicationBuilder UseHclCsCorrelationId(this IApplicationBuilder app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        return app;
    }

    public static IApplicationBuilder UseHclCsRequestObservability(this IApplicationBuilder app)
    {
        app.UseMiddleware<RequestObservabilityMiddleware>();
        return app;
    }

    public static IApplicationBuilder UseHclCsEndpoint(this IApplicationBuilder app)
    {
        EnsureObservabilityPipeline(app);
        app.UseMiddleware<HclCsEndpointMiddleware>();
        return app;
    }

    public static IApplicationBuilder UseHclCsApi(this IApplicationBuilder app)
    {
        EnsureObservabilityPipeline(app);
        app.UseMiddleware<HclCsApiMiddleware>();
        return app;
    }

    public static IEndpointRouteBuilder MapHclCsHealthChecks(
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

        app.UseHclCsCorrelationId();
        app.UseHclCsRequestObservability();
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
