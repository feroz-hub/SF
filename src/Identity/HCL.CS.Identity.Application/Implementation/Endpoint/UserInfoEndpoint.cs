using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using HCL.CS.Domain;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.Entities.Endpoint;
using HCL.CS.Domain.Models.Endpoint;
using HCL.CS.Domain.Models.Endpoint.Request;
using HCL.CS.DomainServices;
using HCL.CS.DomainServices.Infra;
using HCL.CS.DomainServices.Wrappers;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Implementation.Endpoint.Results;
using HCL.CS.Service.Implementation.Endpoint.Specifications;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint.Parsers;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint.Results;

namespace HCL.CS.Service.Implementation.Endpoint;

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
        HclCsConfig settings,
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
