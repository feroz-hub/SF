using System.Net;
using Microsoft.AspNetCore.Http;
using Zentra.Domain;
using Zentra.Domain.Constants;
using Zentra.Domain.Constants.Endpoint;
using Zentra.Domain.ErrorCodes;
using Zentra.DomainServices.Infra;
using Zentra.Service.Implementation.Endpoint.Extensions;
using Zentra.Service.Implementation.Endpoint.Results;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Results;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Validators;

namespace Zentra.Service.Implementation.Endpoint;

internal class TokenRevocationEndpoint : IEndpoint
{
    private readonly IClientSecretValidator clientSecretValidator;
    private readonly ILoggerService loggerService;
    private readonly IResourceStringHandler resourceStringHandler;
    private readonly ITokenRevocationRequestValidator revocationRequestValidator;

    public TokenRevocationEndpoint(
        ILoggerInstance instance,
        IClientSecretValidator clientSecretValidator,
        ITokenRevocationRequestValidator revocationRequestValidator,
        IResourceStringHandler resourceStringHandler)
    {
        this.clientSecretValidator = clientSecretValidator;
        this.revocationRequestValidator = revocationRequestValidator;
        loggerService = instance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
        this.resourceStringHandler = resourceStringHandler;
    }

    public async Task<IEndpointResult> ProcessAsync(HttpContext context)
    {
        if (!HttpMethods.IsPost(context.Request.Method) || !context.Request.CheckHeaderContentType())
        {
            loggerService.WriteTo(Log.Debug, "Invalid HTTP request for token revocation endpoint.");
            return OpenIdConstants.Errors.InvalidRequest.Error(
                resourceStringHandler.GetResourceString(EndpointErrorCodes.InvalidHttpRequest));
        }

        // validate client
        var clientResult = await clientSecretValidator.ValidateClientSecretAsync(context);
        if (clientResult.Client == null) return OpenIdConstants.Errors.InvalidClient.Error();

        loggerService.WriteTo(Log.Debug, "Processing token revocation request.");

        // validate request
        var requestCollection = (await context.Request.ReadFormAsync()).ConvertCollection();
        var requestResult =
            await revocationRequestValidator.ValidateRevocationRequestAsync(requestCollection, clientResult.Client);

        if (requestResult.IsError) return requestResult.ErrorCode.Error();

        return new StatusCodeResult(HttpStatusCode.OK);
    }
}
