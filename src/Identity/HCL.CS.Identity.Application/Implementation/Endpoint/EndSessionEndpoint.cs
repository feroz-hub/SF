/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.AspNetCore.Http;
using HCL.CS.Domain;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.DomainServices.Infra;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Implementation.Endpoint.Results;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint.Results;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint.Validators;

namespace HCL.CS.Service.Implementation.Endpoint;

internal class EndSessionEndpoint : IEndpoint
{
    private readonly IEndSessionRequestValidator endSessionRequestValidator;
    private readonly ILoggerService loggerService;
    private readonly IResourceStringHandler resourceStringHandler;
    private readonly ISessionManagementService sessionManagement;

    public EndSessionEndpoint(
        ILoggerInstance instance,
        IEndSessionRequestValidator endSessionRequestValidator,
        IResourceStringHandler resourceStringHandler,
        ISessionManagementService sessionManagement)
    {
        this.endSessionRequestValidator = endSessionRequestValidator;
        loggerService = instance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
        this.sessionManagement = sessionManagement;
        this.resourceStringHandler = resourceStringHandler;
    }

    public async Task<IEndpointResult> ProcessAsync(HttpContext context)
    {
        loggerService.WriteTo(Log.Debug, "Processing logout Session request.");
        Dictionary<string, string> requestCollection = null;
        if (HttpMethods.IsGet(context.Request.Method))
        {
            requestCollection = context.Request.Query.ConvertCollection();
        }
        else if (HttpMethods.IsPost(context.Request.Method))
        {
            requestCollection = (await context.Request.ReadFormAsync()).ConvertCollection();
        }
        else
        {
            loggerService.WriteTo(Log.Error, "Invalid HTTP request for Session endpoint.");
            return OpenIdConstants.Errors.InvalidRequest.Error(
                resourceStringHandler.GetResourceString(EndpointErrorCodes.InvalidHttpRequest));
        }

        var user = await sessionManagement.GetUserPrincipalFromContextAsync();
        var result = await endSessionRequestValidator.ValidateRequestAsync(requestCollection, user);
        if (result.IsError)
            loggerService.WriteTo(Log.Error, "Error processing end session request.");
        else
            loggerService.WriteTo(Log.Debug, "Success validating end session request.");

        return new EndSessionResult(result);
    }
}
