using AutoMapper;
using Microsoft.AspNetCore.Http;
using Zentra.Domain;
using Zentra.Domain.Constants;
using Zentra.Domain.Constants.Endpoint;
using Zentra.Domain.Entities.Endpoint;
using Zentra.Domain.Enums;
using Zentra.Domain.ErrorCodes;
using Zentra.Domain.Models.Endpoint;
using Zentra.Domain.Models.Endpoint.Validation;
using Zentra.DomainServices;
using Zentra.DomainServices.Infra;
using Zentra.Service.Implementation.Endpoint.Extensions;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Parsers;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Validators;

namespace Zentra.Service.Implementation.Endpoint.Validators;

internal class ClientSecretValidator : IClientSecretValidator
{
    private readonly IRepository<Clients> clientRepository;
    private readonly IClientSecretParser clientSecretParser;
    private readonly ILoggerService loggerService;
    private readonly IMapper mapper;
    private readonly IResourceStringHandler resourceStringHandler;
    private readonly ISecretValidator secretValidator;

    public ClientSecretValidator(
        ILoggerInstance instance,
        IClientSecretParser clientSecretParser,
        IRepository<Clients> clientRepository,
        ISecretValidator secretValidator,
        IResourceStringHandler resourceStringHandler,
        IMapper mapper)
    {
        this.clientSecretParser = clientSecretParser;
        this.clientRepository = clientRepository;
        this.secretValidator = secretValidator;
        loggerService = instance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
        this.resourceStringHandler = resourceStringHandler;
        this.mapper = mapper;
    }

    public async Task<ClientSecretValidationModel> ValidateClientSecretAsync(HttpContext context)
    {
        loggerService.WriteTo(Log.Debug, "Entered into validate client secret.");

        var clientValidationResult = new ClientSecretValidationModel();

        var parsedSecret = await clientSecretParser.ParseAsync(context);
        if (parsedSecret == null || parsedSecret.IsError)
        {
            loggerService.WriteTo(Log.Error, "Client credentials not found.");
            clientValidationResult.ErrorDescription = "Client credentials not found.";
            return clientValidationResult;
        }

        // load client
        ClientsModel client = null;
        var clientsEntity = await clientRepository.GetAsync(
            client =>
                client.ClientId == parsedSecret.ClientId,
            new System.Linq.Expressions.Expression<Func<Zentra.Domain.Entities.Endpoint.Clients, object>>[] { x => x.RedirectUris, x => x.PostLogoutRedirectUris });
        if (clientsEntity.ContainsAny())
        {
            var clientEntity = clientsEntity.ToList()[0];
            client = mapper.Map<Clients, ClientsModel>(clientEntity);
        }

        if (client == null)
        {
            loggerService.WriteTo(Log.Error, "Invalid client identifier.");
            clientValidationResult.ErrorDescription = "Invalid client identifier.";
            return clientValidationResult;
        }

        if (string.IsNullOrWhiteSpace(client.AllowedSigningAlgorithm))
            client.AllowedSigningAlgorithm = OpenIdConstants.Algorithms.RsaSha256;

        var supportedSigningAlgorithm =
            string.Equals(client.AllowedSigningAlgorithm, OpenIdConstants.Algorithms.RsaSha256,
                StringComparison.Ordinal)
            || string.Equals(client.AllowedSigningAlgorithm, OpenIdConstants.Algorithms.EcdsaSha256,
                StringComparison.Ordinal);
        if (!supportedSigningAlgorithm)
        {
            loggerService.WriteTo(Log.Error, "Client must use RS256 or ES256 signing algorithm. Client: {clientId}.",
                client.ClientId);
            clientValidationResult.ErrorDescription =
                resourceStringHandler.GetResourceString(EndpointErrorCodes.SigningAlgorithmIsInvalid);
            return clientValidationResult;
        }

        if (client.RequireClientSecret)
        {
            if (parsedSecret.Type == AuthenticationConstants.ParsedTypes.NoSecret || parsedSecret.Credential == null)
            {
                loggerService.WriteTo(Log.Error, "Client secret required but not provided for client: {clientId}.",
                    client.ClientId);
                clientValidationResult.ErrorDescription =
                    resourceStringHandler.GetResourceString(EndpointErrorCodes.ClientSecretInvalid);
                return clientValidationResult;
            }

            if (parsedSecret.ParseMethod != ParseMethods.Basic &&
                parsedSecret.ParseMethod != ParseMethods.Post)
            {
                loggerService.WriteTo(Log.Error,
                    "Confidential client authentication must use client_secret_basic or client_secret_post. Client: {clientId}.",
                    client.ClientId);
                clientValidationResult.ErrorDescription =
                    resourceStringHandler.GetResourceString(EndpointErrorCodes.ClientSecretInvalid);
                return clientValidationResult;
            }

            var isValid = await secretValidator.ValidateSecretAsync(client, parsedSecret);
            if (!isValid)
            {
                loggerService.WriteTo(Log.Error, "Client secret validation failed for client: {clientId}.",
                    client.ClientId);
                clientValidationResult.ErrorDescription =
                    resourceStringHandler.GetResourceString(EndpointErrorCodes.ClientSecretInvalid);
                return clientValidationResult;
            }
        }
        else
        {
            loggerService.WriteTo(Log.Debug, "Public client - skipping client secret validation.");
        }

        clientValidationResult = new ClientSecretValidationModel
        {
            IsError = false,
            Client = client,
            Secret = parsedSecret
        };
        return clientValidationResult;
    }
}
