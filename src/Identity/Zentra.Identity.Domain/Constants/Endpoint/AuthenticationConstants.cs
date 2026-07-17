using static Zentra.Domain.Constants.Endpoint.OpenIdConstants;
using ClaimTypes = System.Security.Claims.ClaimTypes;

//TODO - Remove unused constants
namespace Zentra.Domain.Constants.Endpoint;

public static class AuthenticationConstants
{
    public enum ScopeRequirement
    {
        None,
        ResourceOnly,
        IdentityOnly,
        Identity
    }

    public const string ZentraName = "Zentra";
    public const string ZentraType = ZentraName;
    public const string ExternalAuthenticationMethod = "external";
    public const string DefaultHashAlgorithm = "SHA256";

    public const int DefaultVerificationCodeExpiryDuration = 60;

    public const string LocalIdentityProvider = "local";
    //public const string DefaultCookieAuthenticationScheme = "zentra.Identity";
    //public const string SignoutScheme = "zentra.Identity";
    //public const string ExternalCookieAuthenticationScheme = "zentra.External";

    public const int QueryStringLength = 128;

    public const int KeySize16 = 16;
    public const int KeySize24 = 24;
    public const int KeySize32 = 32;

    public static readonly TimeSpan DefaultCookieTimeSpan = TimeSpan.FromHours(10);
    public static readonly TimeSpan DefaultCacheDuration = TimeSpan.FromMinutes(60);

    public static readonly List<string> CodeChallengeMethods = new()
    {
        OpenIdConstants.CodeChallengeMethods.Sha256
    };

    public static readonly List<string> AllowedResponseTypes = new()
    {
        ResponseTypes.Code
    };

    public static readonly Dictionary<string, string> AllowedGrantTypeForResponseType = new()
    {
        { ResponseTypes.Code, GrantType.AuthorizationCode }
    };

    public static readonly Dictionary<string, IEnumerable<string>> AllowedResponseModesForGrantType = new()
    {
        { GrantType.AuthorizationCode, new[] { ResponseModes.Query, ResponseModes.FormPost } }
    };

    public static readonly List<string> AllowedResponseModes = new()
    {
        ResponseModes.FormPost,
        ResponseModes.Query
    };

    public static readonly List<string> AllowedGrantTypesForAuthorizeEndpoint = new()
    {
        GrantType.AuthorizationCode
    };

    public static readonly List<string> AllowedPromptModes = new()
    {
        PromptModes.None,
        PromptModes.Login,
        PromptModes.SelectAccount
    };

    public static readonly Dictionary<string, ScopeRequirement> ResponseTypeToScope = new()
    {
        { ResponseTypes.Code, ScopeRequirement.Identity },
        { ResponseTypes.Token, ScopeRequirement.ResourceOnly },
        { ResponseTypes.IdToken, ScopeRequirement.IdentityOnly },
        { ResponseTypes.IdTokenToken, ScopeRequirement.Identity },
        { ResponseTypes.CodeIdToken, ScopeRequirement.Identity },
        { ResponseTypes.CodeToken, ScopeRequirement.Identity },
        { ResponseTypes.CodeIdTokenToken, ScopeRequirement.Identity }
    };

    public static readonly Dictionary<string, string> StandardClaims = new()
    {
        { "username", ClaimTypes.Name },
        { "family_name", ClaimTypes.Surname },
        { "given_name", ClaimTypes.GivenName },
        { "gender", ClaimTypes.Gender },
        { "pincode", ClaimTypes.PostalCode },
        { "dateofbirth", ClaimTypes.DateOfBirth },
        { "email", ClaimTypes.Email },
        { "street ", ClaimTypes.StreetAddress }
    };

    public static class EnvironmentPaths
    {
        public const string ZentraBasePath = "zentra:ZentraServerBasePath";
        public const string SignOutCalled = "zentra:ZentraServerSignOutCalled";
    }

    public static class ParsedTypes
    {
        public const string NoSecret = "NoSecret";
        public const string SharedSecret = "SharedSecret";
        public const string JwtBearer = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";
    }

    public static class SecretTypes
    {
        public const string SharedSecret = "SharedSecret";
        public const string JsonWebKey = "JWK";
        public const string X509Certificate = "X509Certificate";
    }

    public static class ApplicationUIConstants
    {
        // the limit after which old messages are purged
        public const int CookieMessageThreshold = 2;

        public static class DefaultRoutePathParams
        {
            public const string Error = "errorId";

            public const string Login = "returnUrl";

            //public const string Consent = "returnUrl";
            public const string Logout = "logoutId";

            public const string EndSessionCallback = "endSessionId";
            //public const string Custom = "returnUrl";
            //public const string UserCode = "userCode";
        }

        public static class DefaultRoutePaths
        {
            public const string Login = "/account/login";

            public const string Logout = "/account/logout";

            //public const string Consent = "/consent";
            public const string Error = "/home/error";
            //public const string DeviceVerification = "/device";
        }
    }

    public static class GrantType
    {
        public const string Hybrid = "hybrid";
        public const string AuthorizationCode = "authorization_code";
        public const string ClientCredentials = "client_credentials";

        public const string ResourceOwnerPassword = "password";
        public const string UserCode = "user_code";
        //public const string DeviceFlow = "urn:ietf:params:oauth:grant-type:device_code";
    }

    public static class ProtocolTypes
    {
        public const string OpenIdConnect = "oidc";
        //public const string WsFederation = "wsfed";
        //public const string Saml2p = "saml2p";
    }

    public static class IdentityScopes
    {
        public const string OpenId = "openid";
        public const string Profile = "profile";
        public const string Email = "email";
        public const string Address = "address";
        public const string Phone = "phone";
        public const string OfflineAccess = "offline_access";
    }

    public static class AuthCodeStore
    {
        public const string AuthCodeRequestObjectName = "authzId";
        public const string ReturnUrlCode = "returnUrlId";
        public const string UserVerificationCode = "UserCode";
    }

    public class AccessTokenFilters
    {
        // filter for claims from an incoming access token (e.g. used at the user profile endpoint)
        public static readonly string[] ClaimsFilter =
        {
            OpenIdConstants.ClaimTypes.AccessTokenHash,
            OpenIdConstants.ClaimTypes.Audience,
            OpenIdConstants.ClaimTypes.AuthorizedParty,
            OpenIdConstants.ClaimTypes.AuthorizationCodeHash,
            OpenIdConstants.ClaimTypes.ClientId,
            OpenIdConstants.ClaimTypes.Expiration,
            OpenIdConstants.ClaimTypes.IssuedAt,
            OpenIdConstants.ClaimTypes.Issuer,
            OpenIdConstants.ClaimTypes.JwtId,
            OpenIdConstants.ClaimTypes.Nonce,
            OpenIdConstants.ClaimTypes.NotBefore,
            OpenIdConstants.ClaimTypes.ReferenceTokenId,
            OpenIdConstants.ClaimTypes.SessionId,
            OpenIdConstants.ClaimTypes.Scope
        };
    }

    public static class CurveOids
    {
        public const string P256 = "1.2.840.10045.3.1.7";
        public const string P384 = "1.3.132.0.34";
        public const string P521 = "1.3.132.0.35";
    }
}
