using Microsoft.AspNetCore.Http;
using Zentra.DomainServices.Infra;

namespace Zentra.Infrastructure.Services.Implementation;

internal class HttpTenantContext : ITenantContext
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public HttpTenantContext(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public string TenantId
    {
        get
        {
            var context = httpContextAccessor.HttpContext;
            if (context == null) return string.Empty;

            var headerValue = context.Request.Headers["X-Tenant-Id"].ToString();
            if (!string.IsNullOrWhiteSpace(headerValue)) return headerValue;

            var claimValue = context.User?.FindFirst("tenant_id")?.Value
                             ?? context.User?.FindFirst("tenant")?.Value
                             ?? context.User?.FindFirst("tid")?.Value;

            return claimValue ?? string.Empty;
        }
    }
}
