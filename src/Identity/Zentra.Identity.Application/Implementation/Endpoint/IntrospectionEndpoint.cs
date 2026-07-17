using System.Net;
using Microsoft.AspNetCore.Http;
using Zentra.Domain;
using Zentra.Domain.Constants;
using Zentra.Domain.Constants.Endpoint;
using Zentra.Domain.Entities.Api;
using Zentra.Domain.Entities.Endpoint;
using Zentra.Domain.ErrorCodes;
using Zentra.Domain.Models.Endpoint.Request;
using Zentra.Domain.Models.Endpoint.Response;
using Zentra.DomainServices;
using Zentra.DomainServices.Infra;
using Zentra.DomainServices.Wrappers;
using Zentra.Service.Extension;
using Zentra.Service.Implementation.Endpoint.Extensions;
using Zentra.Service.Implementation.Endpoint.Results;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Results;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Validators;

namespace Zentra.Service.Implementation.Endpoint;

internal class IntrospectionEndpoint : IEndpoint
{
    private readonly IRepository<Clients> clientRepository;
    private readonly IClientSecretValidator clientSecretValidator;
    private readonly IIntrospectionRequestValidator introspectionRequestValidator;
    private readonly ILoggerService loggerService;
    private readonly IResourceStringHandler resourceStringHandler;
    private readonly UserManagerWrapper<Users> userManager;

    public IntrospectionEndpoint(
        ILoggerInstance instance,
        IIntrospectionRequestValidator introspectionRequestValidator,
        IClientSecretValidator clientSecretValidator,
        UserManagerWrapper<Users> userManager,
        IResourceStringHandler resourceStringHandler,
        IRepository<Clients> clientRepository)
    {
        this.introspectionRequestValidator = introspectionRequestValidator;
        this.clientSecretValidator = clientSecretValidator;
        loggerService = instance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
        this.userManager = userManager;
        this.resourceStringHandler = resourceStringHandler;
        this.clientRepository = clientRepository;
    }

    public async Task<IEndpointResult> ProcessAsync(HttpContext context)
    {
        loggerService.WriteTo(Log.Debug, "Starting introspection request.");

        if (!HttpMethods.IsPost(context.Request.Method))
        {
            loggerService.WriteTo(Log.Error, "Invalid HTTP request for token endpoint.");
            return new StatusCodeResult(HttpStatusCode.MethodNotAllowed);
        }

        if (!context.Request.CheckHeaderContentType())
        {
            loggerService.WriteTo(Log.Error, "Invalid HTTP request for token endpoint.");
            return new StatusCodeResult(HttpStatusCode.UnsupportedMediaType);
        }

        var validationResult = new ValidatedIntrospectionRequestModel();

        var requestCollection = (await context.Request.ReadFormAsync()).ConvertCollection();

        // validate client
        var clientResult = await clientSecretValidator.ValidateClientSecretAsync(context);
        if (clientResult.Client == null)
        {
            loggerService.WriteTo(Log.Error, "Client not found.");
            validationResult.ErrorCode = OpenIdConstants.Errors.InvalidClient;
            return validationResult.ErrorCode.Error(
                resourceStringHandler.GetResourceString(EndpointErrorCodes.ClientDoesNotExist));
        }

        // request validation
        loggerService.WriteTo(Log.Debug, "Calling into introspection request validator: {type}",
            introspectionRequestValidator.GetType().FullName);

        validationResult =
            await introspectionRequestValidator.ValidateIntrospectionRequestAsync(requestCollection,
                clientResult.Client);

        // create response object.
        var response = new IntrospectionResponseModel();
        switch (validationResult.IsError)
        {
            // render result
            case false:
            {
                response.Active = true;
                if (validationResult.TokenType == OpenIdConstants.TokenResponseType.AccessToken)
                {
                    Users user = null;
                    response.ClientId = validationResult.ClientId;
                    if (!string.IsNullOrWhiteSpace(validationResult.UserId) && validationResult.UserId.IsGuid())
                    {
                        user = await userManager.FindByIdAsync(validationResult.UserId);
                        if (user != null)
                        {
                            response.UserName = user.UserName;
                            response.SubjectId = user.Id.ToString();
                        }
                        else
                        {
                            var clientsEntity =
                                await clientRepository.GetAsync(client => client.ClientId == validationResult.UserId);
                            if (clientsEntity.ContainsAny())
                            {
                                response.UserName = clientsEntity.ToList()[0].ClientName;
                                if (string.IsNullOrWhiteSpace(response.ClientId))
                                    response.ClientId = validationResult.UserId;
                            }
                        }
                    }

                    response.Audience = validationResult.DecodedToken.Audiences.ConvertSpaceSeparatedString();
                    response.Issuer = validationResult.DecodedToken.Issuer;
                    response.Scope = validationResult.Scopes;
                    response.IssuedAt = validationResult.DecodedToken.IssuedAt.ToUnixTime().ToString();
                    response.ExpiresAt = validationResult.ExpiresAt.HasValue
                        ? validationResult.ExpiresAt.ToString()
                        : string.Empty;
                }
                else
                {
                    response.IssuedAt = validationResult.IssuedAt.ToString();
                    response.ExpiresAt = validationResult.ExpiresAt.HasValue
                        ? validationResult.ExpiresAt.ToString()
                        : string.Empty;
                }

                loggerService.WriteTo(Log.Debug, validationResult.Active.ToString());
                break;
            }

            case true:
                response.Active = false;
                loggerService.WriteTo(Log.Debug, "The token is not active." + validationResult.ErrorCode);
                return new IntrospectionResult(response);
        }

        loggerService.WriteTo(Log.Debug, "Introspection request processed successfully.");
        return new IntrospectionResult(response);
    }
}
