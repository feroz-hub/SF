/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using HCL.CS.Domain;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.DomainServices.Infra;
using HCL.CS.ProxyService.Extension;
using HCL.CS.Service.Interfaces.Interfaces.Api.Wrapper;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint;

namespace HCL.CS.ProxyService.Validator;

public sealed class ApiValidator(
    IHttpContextAccessor httpContextAccessor,
    IFrameworkResultService frameworkResultService,
    ISessionManagementService sessionManagementService)
    : IApiValidator
{
    private readonly ISessionManagementService sessionManagementService = sessionManagementService;

    public async Task<FrameworkResult> ValidateRequest(
        [CallerMemberName] string callerMemberName = null)
    {
        if (!ProxyConstants.AnonymousApis.Contains(callerMemberName))
            // Check is user has permission to access api.
            return await IsUserHasPermission(callerMemberName);

        return frameworkResultService.Succeeded();
    }

    private Task<FrameworkResult> IsUserHasPermission(string callerMemberName)
    {
        var api = ApiRoutePathConstants.ApiRouteModels.Find(x => x.Name == callerMemberName);
        if (api != null && api.Permissions != null && api.Permissions.Count > 0)
        {
            var anonymousPermission =
                api.Permissions.Where(x => x.Contains(ApiPermissionConstants.Anonymous)).ToList();
            if (anonymousPermission.Count > 0)
                // Requested api is anonymous, hence allowing permission.
                return Task.FromResult(frameworkResultService.Succeeded());
        }
        else
        {
            return Task.FromResult(frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.NotFound));
        }

        // Check is user has permission to access api.
        var authorization = httpContextAccessor.HttpContext?.Request?.Headers[HeaderNames.Authorization];
        if (!AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            return Task.FromResult(
                frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.UnauthorizedAccess));

        var parsedToken = new JwtSecurityToken(headerValue.Parameter);
        if (parsedToken.Claims == null) return Task.FromResult(frameworkResultService.Succeeded());

        var tokenClaims = new HashSet<string>(
            parsedToken.Claims.Select(x => x.Value.ToLowerInvariant()),
            StringComparer.Ordinal);
        var scopelist = parsedToken.Claims
            .Where(x => x.Type == OpenIdConstants.ClaimTypes.Scope ||
                        x.Type == OpenIdConstants.ClaimTypes.Permission).Select(x => x.Value).ToList();
        if (scopelist.Count > 0)
        {
            var parsedScopes = scopelist[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Distinct();
            foreach (var scope in parsedScopes) tokenClaims.Add(scope.ToLowerInvariant());
        }

        var permissionsList = api.Permissions.ExpandPermissions();
        var claimsList = tokenClaims.ToList().ExpandPermissions();

        var permissions = permissionsList.Intersect(claimsList).ToList();
        if (permissions.Count <= 0)
            return Task.FromResult(frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.AccessDenied));

        return Task.FromResult(frameworkResultService.Succeeded());
    }
}
