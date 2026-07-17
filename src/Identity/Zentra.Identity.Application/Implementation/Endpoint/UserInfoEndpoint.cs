using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Zentra.Domain;
using Zentra.Domain.Constants;
using Zentra.Domain.Constants.Endpoint;
using Zentra.Domain.Entities.Api;
using Zentra.Domain.Entities.Endpoint;
using Zentra.Domain.Models.Endpoint;
using Zentra.Domain.Models.Endpoint.Request;
using Zentra.DomainServices;
using Zentra.DomainServices.Infra;
using Zentra.DomainServices.Wrappers;
using Zentra.Service.Implementation.Endpoint.Extensions;
using Zentra.Service.Implementation.Endpoint.Results;
using Zentra.Service.Implementation.Endpoint.Specifications;
using Zentra.Service.Interfaces.Interfaces.Endpoint;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Parsers;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Results;

namespace Zentra.Service.Implementation.Endpoint;

internal class UserInfoEndpoint : IEndpoint
{
    private readonly IRepository<Clients> clientRepository;
    private readonly TokenSettings configSettings;
    private readonly IFrameworkResultService frameworkResultService;
    private readonly Dictionary<string, AsymmetricKeyInfoModel> keyStore;
    private readonly ILoggerService loggerService;
    private readonly IMapper mapper;
    private readonly IRepository<SecurityTokens> securityTokenRepository;
    private readonly ITokenParser tokenParser;
    private readonly IUserInfoServices userInfoServices;
    private readonly UserManagerWrapper<Users> userManager;

    public UserInfoEndpoint(
        ILoggerInstance instance,
        ITokenParser tokenParser,
        IRepository<Clients> clientRepository,
        IMapper mapper,
        UserManagerWrapper<Users> userManager,
        Dictionary<string, AsymmetricKeyInfoModel> keyStore,
        IUserInfoServices userInfoServices,
        IFrameworkResultService frameworkResultService,
        ZentraConfig settings,
        IRepository<SecurityTokens> securityTokenRepository)
    {
        this.tokenParser = tokenParser;
        this.clientRepository = clientRepository;
        this.mapper = mapper;
        this.userManager = userManager;
        this.keyStore = keyStore ?? throw new ArgumentNullException(nameof(keyStore));
        this.userInfoServices = userInfoServices;
        configSettings = settings.TokenSettings;
        this.frameworkResultService = frameworkResultService;
        this.securityTokenRepository = securityTokenRepository;
        loggerService = instance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
    }

    public async Task<IEndpointResult> ProcessAsync(HttpContext context)
    {
        loggerService.WriteTo(Log.Debug, "Processing user information request.");

        if (!HttpMethods.IsGet(context.Request.Method) && !HttpMethods.IsPost(context.Request.Method))
        {
            loggerService.WriteTo(Log.Debug, "Invalid HTTP request for userinfo endpoint.");
            return new StatusCodeResult(HttpStatusCode.MethodNotAllowed);
        }

        // validate access token
        var token = await tokenParser.ParseAsync(context);
        if (string.IsNullOrWhiteSpace(token))
            return OpenIdConstants.Errors.InvalidToken.UserInfoError("No access token found.");

        // validate request
        var userInfoRequestValidation = new ValidatedUserInfoRequestModel
        {
            Token = token,
            TokenConfigOptions = configSettings
        };

        var requestValidator = new UserInfoRequestSpecification(clientRepository, mapper, keyStore, userManager,
            configSettings, securityTokenRepository);
        var validationResult = await requestValidator.ValidateAsync(userInfoRequestValidation);
        if (!requestValidator.IsValid)
        {
            validationResult = frameworkResultService.Failed(validationResult.ErrorCode, validationResult.ErrorMessage);
            return validationResult.ErrorCode.UserInfoError(validationResult.ErrorMessage);
        }

        // create response
        var collection = await userInfoServices.ProcessUserInfoAsync(userInfoRequestValidation);

        // return result
        return new UserInfoResult(collection, null);
    }
}
