using Microsoft.AspNetCore.Http;
using Zentra.Domain;
using Zentra.Domain.Constants;
using Zentra.Domain.Constants.Endpoint;
using Zentra.Domain.ErrorCodes;
using Zentra.DomainServices.Infra;
using Zentra.Service.Implementation.Endpoint.Extensions;
using Zentra.Service.Implementation.Endpoint.Results;
using Zentra.Service.Interfaces.Interfaces.Endpoint;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Results;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Validators;

namespace Zentra.Service.Implementation.Endpoint;

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
