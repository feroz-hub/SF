using Microsoft.AspNetCore.Http;
using Zentra.Domain;
using Zentra.Domain.Constants.Endpoint;
using Zentra.Domain.Models.Endpoint.Request;
using Zentra.Service.Extension;
using Zentra.Service.Implementation.Endpoint.Extensions;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Results;

namespace Zentra.Service.Implementation.Endpoint.Results;

internal class NavigationPageResult : IEndpointResult
{
    private readonly TokenSettings configSettings;
    private readonly Guid returnUrlId;

    public NavigationPageResult(ValidatedAuthorizeRequestModel request, TokenSettings tokenConfig, Guid returnUrlId)
    {
        AuthorizeRequest = request;
        configSettings = tokenConfig;
        this.returnUrlId = returnUrlId;
    }

    private ValidatedAuthorizeRequestModel AuthorizeRequest { get; }

    public Task ConstructResponseAsync(HttpContext context)
    {
        var returnUrl = "/" + OpenIdConstants.EndpointRoutePaths.AuthorizeCallback;
        if (returnUrlId.IsValid())
            returnUrl = returnUrl.AddQueryString(AuthenticationConstants.AuthCodeStore.ReturnUrlCode,
                Convert.ToString(returnUrlId));
        else
            returnUrl = returnUrl.FormatQueryString(AuthorizeRequest.RequestRawData.PrepareQueryString());

        var loginUrl = configSettings.UserInteractionConfig.LoginUrl;
        if (!loginUrl.CheckLocalUrl())
            // this converts the relative redirect path to an absolute one if we're
            // redirecting to a different server
            returnUrl = context.GetZentraHost().IncludeEndSlash() + returnUrl.RemoveFrontSlash();

        var url = loginUrl.AddQueryString(configSettings.UserInteractionConfig.LoginReturnUrlParameter, returnUrl);
        context.Response.RedirectToAbsoluteUrl(url);
        return Task.CompletedTask;
    }
}
