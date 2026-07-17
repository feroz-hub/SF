using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Zentra.Domain;
using Zentra.Domain.Constants;
using Zentra.Domain.Constants.Endpoint;
using Zentra.Domain.ErrorCodes;
using Zentra.Domain.Models.Endpoint;
using Zentra.DomainServices.Infra;
using Zentra.Service.Implementation.Endpoint.Extensions;
using Zentra.Service.Interfaces.Interfaces.Endpoint;

namespace Zentra.Service.Implementation.Endpoint.Services;

internal class SessionManagementService : SecurityBase, ISessionManagementService
{
    private const string ClientListKey = "client_list";
    private readonly TokenSettings configSettings;
    private readonly IFrameworkResultService frameworkResultService;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ILoggerService loggerService;

    public SessionManagementService(
        IHttpContextAccessor httpContextAccessor,
        IFrameworkResultService frameworkResultService,
        IAuthenticationHandlerProvider authenticationHandler,
        IAuthenticationSchemeProvider authenticationScheme,
        ILoggerInstance instance,
        ZentraConfig tokenSettings)
    {
        this.frameworkResultService = frameworkResultService;
        configSettings = tokenSettings.TokenSettings;
        this.httpContextAccessor = httpContextAccessor;
        AuthenticationHandler = authenticationHandler;
        AuthenticationScheme = authenticationScheme;
        loggerService = instance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
    }

    public IAuthenticationSchemeProvider AuthenticationScheme { get; set; }

    public IAuthenticationHandlerProvider AuthenticationHandler { get; set; }

    public ClaimsPrincipal Principal { get; set; }

    public AuthenticationProperties Properties { get; set; }

    public virtual async Task<ClaimsPrincipal> GetUserPrincipalFromContextAsync()
    {
        var authPrinciple = await GetAuthenticationAsync();
        return authPrinciple?.Principal;
    }

    public virtual async Task<AuthenticationProperties> GetPropertiesFromContextAsync()
    {
        var authProperties = await GetAuthenticationAsync();
        return authProperties?.Properties;
    }

    public virtual async Task<AuthenticationPropertiesModel> GetAuthenticationAsync()
    {
        if (Principal == null || Properties == null)
        {
            var authenticationScheme = await GetAuthenticationSchemeAsync();
            if (authenticationScheme == null)
                frameworkResultService.Throw(ApiErrorCodes.NoDefaultAuthenticateSchemeFound);

            var handler =
                await AuthenticationHandler.GetHandlerAsync(httpContextAccessor.HttpContext, authenticationScheme);
            if (handler == null) frameworkResultService.Throw(ApiErrorCodes.NoAuthenticationHandlerConfigured);

            var result = await handler.AuthenticateAsync();
            if (result != null && result.Succeeded)
            {
                Principal = result.Principal;
                Properties = result.Properties;
                return new AuthenticationPropertiesModel
                    { Principal = Principal, Properties = Properties, IsError = false };
            }
        }
        else
        {
            return new AuthenticationPropertiesModel
                { Principal = Principal, Properties = Properties, IsError = false };
        }

        return null;
    }

    public virtual async Task<string> GetAuthenticationSchemeAsync()
    {
        var httpContext = httpContextAccessor.HttpContext;
        var authenticationSchemeProvider =
            httpContext.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
        var defaultAuthenticationScheme = await authenticationSchemeProvider.GetDefaultAuthenticateSchemeAsync();
        if (defaultAuthenticationScheme == null) return null;

        return defaultAuthenticationScheme.Name;
    }

    public async Task<string> GetSessionId()
    {
        var properties = await GetPropertiesFromContextAsync();
        return properties?.GetSessionId();
    }

    public async Task CreateAndBindSessionCookieAsync(ClaimsPrincipal principal, AuthenticationProperties properties)
    {
        if (principal == null || properties == null) frameworkResultService.Throw(EndpointErrorCodes.ArgumentNullError);

        var principleSubjectId = principal.GetSubjectId();
        var user = await GetUserPrincipalFromContextAsync();
        var contextSubjectId = user != null ? user.GetSubjectId() : string.Empty;
        if (properties.GetSessionId() == null || contextSubjectId != principleSubjectId)
        {
            var sessionValue = AuthenticationConstants.KeySize24.RandomString();
            properties.SetSessionId(sessionValue);
        }

        Principal = principal;
        Properties = properties;
    }

    public async Task AddClientAsync(string clientId)
    {
        if (clientId == null) frameworkResultService.Throw(EndpointErrorCodes.ArgumentNullError);

        var authenticationModel = await GetAuthenticationAsync();
        if (authenticationModel.Properties != null)
        {
            loggerService.WriteTo(Log.Debug, "Entered into add client : " + clientId);
            var clientList = GetClientCollection(authenticationModel.Properties);
            if (!clientList.Contains(clientId))
            {
                var collection = clientList.ToList();
                collection.Add(clientId);
                var value = EncryptionExtension.Encode(collection);
                if (value == null)
                    Properties.Items.Remove(ClientListKey);
                else
                    Properties.Items[ClientListKey] = value;
            }
        }

        await UpdateSessionCookie();
    }

    public async Task<IList<string>> GetClientListAsync()
    {
        var authenticationModel = await GetAuthenticationAsync();

        try
        {
            if (authenticationModel != null) return GetClientCollection(authenticationModel.Properties);
        }
        catch (Exception)
        {
            RemoveClientList(Properties);
            await UpdateSessionCookie();
        }

        return new List<string>();
    }

    private static IList<string> GetClientCollection(AuthenticationProperties properties)
    {
        if (properties?.Items.ContainsKey(ClientListKey) == true)
        {
            var value = properties.Items[ClientListKey];
            return EncryptionExtension.DecodeList(value);
        }

        return new List<string>();
    }

    private static void RemoveClientList(AuthenticationProperties properties)
    {
        properties?.Items.Remove(ClientListKey);
    }

    private async Task UpdateSessionCookie()
    {
        var authenticationModel = await GetAuthenticationAsync();

        if (authenticationModel == null || (authenticationModel != null &&
                                            (authenticationModel.Principal == null ||
                                             authenticationModel.Properties == null)))
            frameworkResultService.Throw(EndpointErrorCodes.InvalidOperation);

        var scheme = await GetAuthenticationSchemeAsync();
        loggerService.WriteTo(Log.Debug, "Entered into update session cookie : " + scheme);
        await httpContextAccessor.HttpContext.SignInAsync(scheme, authenticationModel.Principal,
            authenticationModel.Properties);
    }
}
