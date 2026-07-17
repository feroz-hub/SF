using System.Linq.Expressions;
using Zentra.Domain;
using Zentra.Domain.Constants;
using Zentra.Domain.Entities.Api;
using Zentra.Domain.Models.Endpoint;
using Zentra.Domain.Models.Endpoint.Request;
using Zentra.DomainServices.Infra;
using Zentra.DomainServices.Repository.Api;
using Zentra.Service.Implementation.Endpoint.Extensions;
using Zentra.Service.Interfaces.Interfaces.Endpoint;
using static Zentra.Domain.Constants.Endpoint.OpenIdConstants;

namespace Zentra.Service.Implementation.Endpoint.Services;

internal class DiscoveryService : SecurityBase, IDiscoveryService
{
    private readonly IApiResourceRepository apiResourceRepository;
    private readonly TokenSettings configSettings;
    private readonly IIdentityResourceRepository identityResourceRepository;
    private readonly Dictionary<string, AsymmetricKeyInfoModel> keyStore;
    private readonly ILoggerService loggerService;
    private List<string> supportedClaimsList;
    private List<string> supportedScopesList;

    public DiscoveryService(
        ILoggerInstance instance,
        IIdentityResourceRepository identityResourceRepository,
        IApiResourceRepository apiResourceRepository,
        Dictionary<string, AsymmetricKeyInfoModel> keyStore,
        ZentraConfig tokenSettings)
    {
        this.identityResourceRepository = identityResourceRepository;
        this.apiResourceRepository = apiResourceRepository;
        this.keyStore = keyStore;
        configSettings = tokenSettings.TokenSettings;
        loggerService = instance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
        supportedClaimsList = new List<string>();
        supportedScopesList = new List<string>();
    }

    public async Task<Dictionary<string, object>> GenerateDiscoveryMetaData(DiscoveryRequestModel request)
    {
        loggerService.WriteTo(Log.Debug, "Entered into generate discovery metadata.");
        await GetSupportedClaims();
        var tokenAuthMethods = new[]
        {
            AuthenticationMethods.ClientSecretBasic,
            AuthenticationMethods.ClientSecretPost
        };
        var responseTypes = new[]
        {
            ResponseTypes.Code
        };
        var responseModes = new[]
        {
            ResponseModes.Query,
            ResponseModes.FormPost
        };
        var grantTypes = new[]
        {
            GrantTypes.AuthorizationCode,
            GrantTypes.RefreshToken,
            GrantTypes.ClientCredentials,
            GrantTypes.Password,
            GrantTypes.UserCode
        };
        var idTokenSigningAlgorithms = keyStore.Keys
            .Where(algorithm => !string.IsNullOrWhiteSpace(algorithm))
            .Distinct()
            .ToArray();
        if (idTokenSigningAlgorithms.Length == 0) idTokenSigningAlgorithms = new[] { Algorithms.RsaSha256 };

        var codeChallengeMethods = new[]
        {
            CodeChallengeMethods.Sha256
        };

        var metaData = new Dictionary<string, object>
        {
            // Issuer
            { "issuer", configSettings.TokenConfig.IssuerUri },

            // Token endpoint auth methods (RFC 8414)
            { "token_endpoint_auth_methods_supported", tokenAuthMethods },

            // Supported modes
            { "scopes_supported", supportedScopesList.ToArray() },
            { "claims_supported", supportedClaimsList.ToArray() },
            { "response_types_supported", responseTypes },
            { "response_modes_supported", responseModes },
            { "grant_types_supported", grantTypes },
            { "subject_types_supported", new[] { "public" } },

            { "id_token_signing_alg_values_supported", idTokenSigningAlgorithms },
            { "code_challenge_methods_supported", codeChallengeMethods }
        };

        if (configSettings.EndpointsConfig.EnableAuthorizeEndpoint)
            metaData.Add("authorization_endpoint", request.BaseUrl + EndpointRoutePaths.Authorize);

        if (configSettings.EndpointsConfig.EnableTokenEndpoint)
            metaData.Add("token_endpoint", request.BaseUrl + EndpointRoutePaths.Token);

        if (configSettings.EndpointsConfig.EnableIntrospectionEndpoint)
            metaData.Add("introspection_endpoint", request.BaseUrl + EndpointRoutePaths.Introspection);

        if (configSettings.EndpointsConfig.EnableJWKSEndpoint && keyStore.ContainsAny())
            metaData.Add("jwks_uri", request.BaseUrl.RemoveBackSlash() + EndpointRoutePaths.JWKSWebKeys);

        if (configSettings.EndpointsConfig.EnableUserInfoEndpoint)
            metaData.Add("userinfo_endpoint", request.BaseUrl + EndpointRoutePaths.UserInfo);

        if (configSettings.EndpointsConfig.EnableTokenRevocationEndpoint)
            metaData.Add("revocation_endpoint", request.BaseUrl + EndpointRoutePaths.Revocation);

        if (configSettings.EndpointsConfig.EnableEndSessionEndpoint)
            metaData.Add("end_session_endpoint", request.BaseUrl + EndpointRoutePaths.EndSession);

        if (configSettings.EndpointsConfig.FrontchannelLogoutSupported)
            metaData.Add("frontchannel_logout_supported", true);

        if (configSettings.EndpointsConfig.FrontchannelLogoutSessionRequired)
            metaData.Add("frontchannel_logout_session_supported", true);

        if (configSettings.EndpointsConfig.BackchannelLogoutSupported)
            metaData.Add("backchannel_logout_supported", true);

        if (configSettings.EndpointsConfig.BackchannelLogoutSessionRequired)
            metaData.Add("backchannel_logout_session_supported", true);

        return metaData;
    }

    private async Task GetSupportedClaims()
    {
        supportedClaimsList.Clear();
        supportedScopesList.Clear();
        var identityResources = await identityResourceRepository.GetAllAsync(new Expression<Func<IdentityResources, object>>[] { x => x.IdentityClaims });
        if (identityResources.ContainsAny())
            foreach (var identityResource in identityResources)
            {
                supportedClaimsList.AddRange(identityResource.IdentityClaims.ConvertAll(x => x.Type));
                supportedScopesList.Add(identityResource.Name);
            }

        var apiResources = await apiResourceRepository.GetAllApiResourcesAsync();
        if (apiResources.ContainsAny())
            foreach (var apiResource in apiResources)
                supportedClaimsList.AddRange(apiResource.ApiResourceClaims.ConvertAll(x => x.Type));

        var apiScopes = await apiResourceRepository.GetAllApiScopesAsync();
        if (apiScopes.ContainsAny())
            foreach (var apiScope in apiScopes)
            {
                supportedClaimsList.AddRange(apiScope.ApiScopeClaims.ConvertAll(x => x.Type));
                supportedScopesList.Add(apiScope.Name);
            }

        supportedClaimsList = supportedClaimsList.Distinct().ToList();
        supportedScopesList = supportedScopesList.Distinct().ToList();
    }
}
