/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.AspNetCore.Http;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.Models.Endpoint;
using HCL.CS.Domain.Models.Endpoint.Request;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint.Results;

namespace HCL.CS.Service.Implementation.Endpoint.Results;

internal class EndSessionResult : IEndpointResult
{
    private readonly ValidatedEndSessionRequestModel validatedRequest;

    public EndSessionResult(ValidatedEndSessionRequestModel validatedRequest)
    {
        this.validatedRequest = validatedRequest;
    }

    public async Task ConstructResponseAsync(HttpContext context)
    {
        string logoutId = null;
        var request = validatedRequest.IsError ? null : validatedRequest;
        if (request != null)
        {
            var logoutMessage = await PrepareLogoutMessage(validatedRequest);
                if (logoutMessage.HasClient) logoutId = await logoutMessage.ProtectDataAsync();
        }

        var userInteractionConfig = validatedRequest.TokenConfigOptions?.UserInteractionConfig;

        // Always redirect through the internal logout page (e.g. /account/logout) when
        // logoutId is available. This ensures the session cookie is cleared via SignOutAsync
        // before the user is redirected to the client's post_logout_redirect_uri.
        // Previously, when PostLogOutUri was set, the redirect went directly to the client
        // without clearing HCL.CS's auth cookie, causing subsequent logins to fail.
        string redirect;
        if (!string.IsNullOrWhiteSpace(logoutId) && !string.IsNullOrWhiteSpace(userInteractionConfig?.LogoutUrl))
        {
            redirect = userInteractionConfig.LogoutUrl;
        }
        else
        {
            redirect = validatedRequest.PostLogOutUri;
            if (string.IsNullOrWhiteSpace(redirect)) redirect = userInteractionConfig?.LogoutUrl;
            if (string.IsNullOrWhiteSpace(redirect)) redirect = "/";
        }

        if (redirect.IsLocalUrl()) redirect = context.GetHclCsRelativePath(redirect);

        if (!string.IsNullOrWhiteSpace(logoutId) && !string.IsNullOrWhiteSpace(userInteractionConfig?.LogoutIdParameter))
            redirect = redirect.AddQueryString(
                userInteractionConfig.LogoutIdParameter, logoutId);

        context.Response.Redirect(redirect);
    }

    internal static Task<LogoutMessageModel> PrepareLogoutMessage(ValidatedEndSessionRequestModel request)
    {
        var logoutMessage = new LogoutMessageModel();
        if (request != null)
        {
            if (request.RequestRawData != null) logoutMessage.Parameters = request.RequestRawData;

            logoutMessage.ClientId = request.Client?.ClientId;
            logoutMessage.SubjectId = request.SubjectId;
            logoutMessage.SessionId = request.SessionId;
            logoutMessage.ClientIdCollection = request.ClientIds;

            if (request.PostLogOutUri != null)
            {
                logoutMessage.PostLogoutRedirectUri = request.PostLogOutUri;
                if (request.State != null)
                    logoutMessage.PostLogoutRedirectUri =
                        logoutMessage.PostLogoutRedirectUri.AddQueryString(OpenIdConstants.EndSessionRequest.State,
                            request.State);
            }
        }

        return Task.FromResult(logoutMessage);
    }
}
