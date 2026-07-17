using Microsoft.AspNetCore.Http;

namespace Zentra.ProxyService.Hosting;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var inboundCorrelationId = context.Request.Headers[ObservabilityConstants.CorrelationIdHeaderName].ToString();
        var correlationId = ResolveCorrelationId(inboundCorrelationId);

        context.TraceIdentifier = correlationId;
        context.Items[ObservabilityConstants.CorrelationIdItemKey] = correlationId;
        context.Response.Headers[ObservabilityConstants.CorrelationIdHeaderName] = correlationId;

        await next(context);
    }

    private static string ResolveCorrelationId(string? correlationId)
    {
        if (string.IsNullOrWhiteSpace(correlationId)) return CreateCorrelationId();

        var value = correlationId.Trim();
        if (!IsValidCorrelationId(value)) return CreateCorrelationId();

        return value;
    }

    private static string CreateCorrelationId()
    {
        return Guid.NewGuid().ToString("N");
    }

    private static bool IsValidCorrelationId(string correlationId)
    {
        if (correlationId.Length is <= 0 or > 128) return false;

        foreach (var character in correlationId)
        {
            if (char.IsLetterOrDigit(character)) continue;
            if (character is '-' or '_' or '.') continue;
            return false;
        }

        return true;
    }
}
