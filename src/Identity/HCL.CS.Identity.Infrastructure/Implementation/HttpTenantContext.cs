/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.AspNetCore.Http;
using HCL.CS.DomainServices.Infra;

namespace HCL.CS.Infrastructure.Services.Implementation;

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
