using Microsoft.AspNetCore.Http;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.Enums;
using HCL.CS.Domain.Models.Endpoint.Request;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint.Results;

namespace HCL.CS.Service.Implementation.Endpoint.Results;

internal class EndSessionCallbackResult : IEndpointResult
{
    private readonly ValidatedEndSessionCallbackRequestModel request;

    public EndSessionCallbackResult(ValidatedEndSessionCallbackRequestModel request)
    {
        this.request = request;
    }

    public async Task ConstructResponseAsync(HttpContext context)
    {
        if (request.IsError)
        {
            context.Response.StatusCode = OpenIdConstants.HTTPStatusCodes.GetHttpStatusCode(request.ErrorCode);
        }
        else
        {
            context.Response.SetResponseNoCache();
            AddContentSecurityPolicyHeaders(context);

            var html = GenerateFrontchannelIFrameHtml();
            await context.Response.WriteHtmlResponseAsync(html);
        }
    }

    private void AddContentSecurityPolicyHeaders(HttpContext context)
    {
        if (request.TokenConfigOptions.AuthenticationConfig.RequireCspFrameSrcForSignout)
        {
            string frameSources = null;
            var origins = request.FrontChannelLogoutUrls?.Select(x => x.GetOrigin());
            if (origins.ContainsAny()) frameSources = origins.Distinct().Aggregate((x, y) => $"{x} {y}");

            // the hash matches the embedded style element being used below
            context.Response.AddStyleCspHeaders(CspLevel.Two, "sha256-u+OupXgfekP+x/f6rMdoEAspPCYUtca912isERnoEjY=",
                frameSources);
        }
    }

    private string GenerateFrontchannelIFrameHtml()
    {
        string frontchannelIFrameHtml = null;
        if (request.FrontChannelLogoutUrls.ContainsAny())
        {
            var frontchannelUrls = request.FrontChannelLogoutUrls.Select(url => $"<iframe src='{url}'></iframe>");
            frontchannelIFrameHtml = frontchannelUrls.Aggregate((x, y) => x + y);
        }

        return
            $"<!DOCTYPE html><html><style>iframe{{display:none;width:0;height:0;}}</style><body>{frontchannelIFrameHtml}</body></html>";
    }
}
