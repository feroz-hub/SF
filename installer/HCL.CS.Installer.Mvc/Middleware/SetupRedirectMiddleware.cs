/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HclCsInstallerMVC.Application.Abstractions;

namespace HclCsInstallerMVC.Middleware;

public sealed class SetupRedirectMiddleware
{
    private static readonly PathString SetupPath = new("/setup");
    private static readonly PathString HealthPath = new("/health");
    private static readonly PathString InstalledPath = new("/installed");
    private static readonly PathString ErrorPath = new("/error");
    private readonly ILogger<SetupRedirectMiddleware> _logger;

    private readonly RequestDelegate _next;

    public SetupRedirectMiddleware(RequestDelegate next, ILogger<SetupRedirectMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IInstallationGateService installationGateService)
    {
        var path = context.Request.Path;
        if (IsAlwaysAllowedPath(path))
        {
            await _next(context);
            return;
        }

        var isInstalled = await installationGateService.IsInstallationCompletedAsync(context.RequestAborted);
        if (!isInstalled && !path.StartsWithSegments(SetupPath, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("Setup is not completed. Redirecting {Path} to /setup.", path);
            context.Response.Redirect(SetupPath);
            return;
        }

        if (isInstalled && path.StartsWithSegments(SetupPath, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("Setup already completed. Blocking access to {Path}.", path);
            context.Response.Redirect(InstalledPath);
            return;
        }

        await _next(context);
    }

    private static bool IsAlwaysAllowedPath(PathString path)
    {
        if (path.StartsWithSegments(HealthPath, StringComparison.OrdinalIgnoreCase)
            || path.StartsWithSegments(ErrorPath, StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }
}
