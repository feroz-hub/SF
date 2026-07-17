using Microsoft.AspNetCore.Http;
using Zentra.Domain.Constants.Endpoint;
using Zentra.Domain.Models.Endpoint;
using Zentra.Domain.Models.Endpoint.Request;
using Zentra.Service.Implementation.Endpoint.Extensions;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Results;

namespace Zentra.Service.Implementation.Endpoint.Results;

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
        // without clearing Zentra's auth cookie, causing subsequent logins to fail.
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

        if (redirect.IsLocalUrl()) redirect = context.GetZentraRelativePath(redirect);

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
