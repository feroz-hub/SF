/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.AspNetCore.Http;
using HCL.CS.Domain;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.Models.Endpoint.Request;
using HCL.CS.Service.Extension;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint.Results;

namespace HCL.CS.Service.Implementation.Endpoint.Results;

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
            returnUrl = context.GetHclCsHost().IncludeEndSlash() + returnUrl.RemoveFrontSlash();

        var url = loginUrl.AddQueryString(configSettings.UserInteractionConfig.LoginReturnUrlParameter, returnUrl);
        context.Response.RedirectToAbsoluteUrl(url);
        return Task.CompletedTask;
    }
}
