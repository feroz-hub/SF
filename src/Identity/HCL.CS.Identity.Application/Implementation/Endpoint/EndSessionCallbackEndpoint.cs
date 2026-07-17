using Microsoft.AspNetCore.Http;
using HCL.CS.Domain;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.DomainServices.Infra;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Implementation.Endpoint.Results;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint.Results;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint.Validators;

namespace HCL.CS.Service.Implementation.Endpoint;

internal class EndSessionCallbackEndpoint : IEndpoint
{
    private readonly IEndSessionRequestValidator endSessionRequestValidator;
    private readonly ILoggerService loggerService;
    private readonly IResourceStringHandler resourceStringHandler;

    public EndSessionCallbackEndpoint(
        ILoggerInstance instance,
        IEndSessionRequestValidator endSessionRequestValidator,
        IResourceStringHandler resourceStringHandler)
    {
        this.endSessionRequestValidator = endSessionRequestValidator;
        loggerService = instance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
        this.resourceStringHandler = resourceStringHandler;
    }

    public async Task<IEndpointResult> ProcessAsync(HttpContext context)
    {
        if (!HttpMethods.IsGet(context.Request.Method))
        {
            loggerService.WriteTo(Log.Error, "Invalid HTTP request for session callback endpoint.");
            return OpenIdConstants.Errors.InvalidRequest.Error(
                resourceStringHandler.GetResourceString(EndpointErrorCodes.InvalidHttpRequest));
        }

        loggerService.WriteTo(Log.Debug, "Processing sign out callback request");

        // TODO - Need to identify the front channel logout flow (how it works)
        var requestCollection = context.Request.Query.ConvertCollection();
        var result = await endSessionRequestValidator.ValidateCallbackRequestAsync(requestCollection);
        if (!result.IsError)
            loggerService.WriteTo(Log.Debug, "Successful sign out callback.");
        else
            loggerService.WriteTo(Log.Error, "Error validating sign out callback: {error}", result.ErrorCode);

        return new EndSessionCallbackResult(result);
    }
}
