using Microsoft.AspNetCore.Http;

namespace Zentra.ProxyService.Hosting;

/// <summary>
/// Applies enterprise security response headers to every HTTP response.
/// Enforces HSTS, prevents clickjacking, content-type sniffing, and
/// controls referrer information per OWASP and Zero-Trust recommendations.
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            var headers = context.Response.Headers;

            // Prevent clickjacking - deny all framing.
            if (!headers.ContainsKey("X-Frame-Options"))
                headers["X-Frame-Options"] = "DENY";

            // Prevent MIME-type sniffing.
            if (!headers.ContainsKey("X-Content-Type-Options"))
                headers["X-Content-Type-Options"] = "nosniff";

            // Minimal referrer information on cross-origin requests.
            if (!headers.ContainsKey("Referrer-Policy"))
                headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

            // Enforce HTTPS for 1 year; include sub-domains.
            // Only set over HTTPS to avoid HSTS on plain HTTP (which is a spec violation).
            if (context.Request.IsHttps && !headers.ContainsKey("Strict-Transport-Security"))
                headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";

            // Disable legacy browser features and prevent information leakage via APIs.
            if (!headers.ContainsKey("Permissions-Policy"))
                headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=(), payment=()";

            // Remove the Server header to reduce information exposure.
            headers.Remove("Server");

            return Task.CompletedTask;
        });

        await next(context);
    }
}