/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

namespace HCL.CS.Domain.Constants.Endpoint;

public static class OpenIdConstants
{
    public static class ClaimTypes
    {
        public const string Name = System.Security.Claims.ClaimTypes.Name;
        public const string FamilyName = System.Security.Claims.ClaimTypes.Surname;
        public const string GivenName = System.Security.Claims.ClaimTypes.GivenName;
        public const string Gender = System.Security.Claims.ClaimTypes.Gender;
        public const string PinCode = System.Security.Claims.ClaimTypes.PostalCode;
        public const string DateOfBirth = System.Security.Claims.ClaimTypes.DateOfBirth;
        public const string Email = System.Security.Claims.ClaimTypes.Email;
        public const string Street = System.Security.Claims.ClaimTypes.StreetAddress;

        public const string Sub = "sub";
        public const string UserName = "username";
        public const string PreferredUserName = "preferred_username";
        public const string EmailConfirmed = "email_confirmed";
        public const string EmailVerified = "email_verified";
        public const string PhoneNumber = "phone_number";
        public const string PhoneNumberConfirmed = "phone_number_confirmed";
        public const string PhoneNumberVerified = "phone_number_verified";
        public const string Address = "address";
        public const string MiddleName = "middlename";
        public const string NickName = "nickname";
        public const string Birthdate = "birthdate";
        public const string City = "city";
        public const string LastName = "lastname";
        public const string FirstName = "firstname";

        public const string AuthenticationMethod = "amr";
        public const string SessionId = "sid";
        public const string AuthenticationTime = "auth_time";
        public const string AuthorizedParty = "azp";
        public const string AccessTokenHash = "at_hash";
        public const string AuthorizationCodeHash = "c_hash";
        public const string Nonce = "nonce";
        public const string JwtId = "jti";
        public const string ClientId = "client_id";
        public const string Scope = "scope";
        public const string Actor = "act";
        public const string Id = "id";
        public const string IdentityProvider = "idp";
        public const string UserRole = "userrole";
        public const string Role = "role";
        public const string ReferenceTokenId = "reference_token_id";
        public const string IssuedAt = "iat";
        public const string Expiration = "exp";
        public const string NotBefore = "nbf";
        public const string Audience = "aud";
        public const string Issuer = "iss";
        public const string Permission = "permission";
        public const string Transaction = "transaction";
        public const string Events = "events";
        /// <summary>Custom claim type for a dedicated capabilities list in the access token (e.g. "capabilities": ["read:users", "write:orders"]).</summary>
        public const string Capabilities = "capabilities";
    }

    public static class AuthenticationMethods
    {
        public const string None = "none";
        public const string ClientSecretBasic = "client_secret_basic";
        public const string ClientSecretPost = "client_secret_post";
        public const string ClientSecretJwt = "client_secret_jwt";
    }

    public static class EndpointRoutePaths
    {
        public const string ServerPathPrefix = "security";

        public const string Authorize = ServerPathPrefix + "/authorize";
        public const string AuthorizeCallback = Authorize + "/authorizecallback";
        public const string DiscoveryConfiguration = "/.well-known/openid-configuration";
        public const string JWKSWebKeys = DiscoveryConfiguration + "/jwks";
        public const string Token = ServerPathPrefix + "/token";
        public const string Revocation = ServerPathPrefix + "/revocation";
        public const string UserInfo = ServerPathPrefix + "/userinfo";
        public const string Introspection = ServerPathPrefix + "/introspect";
        public const string EndSession = ServerPathPrefix + "/endsession";
        public const string EndSessionCallback = EndSession + "/callback";
        public const string DeviceAuthorization = ServerPathPrefix + "/deviceauthorization";
        public const string Register = ServerPathPrefix + "/register";

        public static readonly string[] CorsPaths =
        {
            DiscoveryConfiguration,
            JWKSWebKeys,
            Token,
            UserInfo,
            Revocation
        };
    }

    public static class EndpointsName
    {
        public const string Authorize = "Authorize";
        public const string Token = "Token";
        public const string Introspection = "Introspection";
        public const string Revocation = "Revocation";
        public const string UserInfo = "Userinfo";
        public const string Discovery = "Discovery";
        public const string EndSession = "Endsession";
    }

    public static class EndSessionRequest
    {
        public const string IdTokenHint = "id_token_hint";
        public const string PostLogoutRedirectUri = "post_logout_redirect_uri";
        public const string State = "state";
        public const string Sid = "sid";
        public const string Issuer = "iss";
    }

    public static class RevocationRequest
    {
        public const string Token = "token";
        public const string TokenTypeHint = "token_type_hint";
    }

    public static class Errors
    {
        public const string InvalidRequest = "invalid_request";
        public const string InvalidClient = "invalid_client";
        public const string InvalidGrant = "invalid_grant";
        public const string UnauthorizedClient = "unauthorized_client";
        public const string UnsupportedGrantType = "unsupported_grant_type";
        public const string UnsupportedResponseType = "unsupported_response_type";
        public const string UnsupportedAlgorithm = "unsupported_algorithm";
        public const string UnsupportedTokenType = "unsupported_token_type";
        public const string InvalidScope = "invalid_scope";
        public const string AuthorizationPending = "authorization_pending";
        public const string AccessDenied = "access_denied";
        public const string SlowDown = "slow_down";
        public const string ExpiredToken = "token_expired";
        public const string InvalidTarget = "invalid_target";
        public const string InvalidEndpoint = "invalid_endpoint";
        public const string InvalidToken = "invalid_token";
        public const string RequestUriNotSupported = "request_uri_not_supported";
        public const string RequestNotSupported = "request_not_supported";
        public const string InvalidRequestObject = "invalid_request_object";
        public const string InvalidRequestUri = "invalid_request_uri";
        public const string ConsentRequired = "consent_required";
        public const string AccountSelectionRequired = "account_selection_required";
        public const string RegistrationNotSupported = "registration_not_supported";
        public const string LoginRequired = "login_required";
        public const string TemporarilyUnavailable = "temporarily_unavailable";
        public const string ServerError = "server_error";
        public const string InteractionRequired = "interaction_required";
        public const string InsufficientScope = "insufficient_scope";
        public const string InvalidFormat = "invalid_format";
    }

    public static class HTTPStatusCodes
    {
        public const int success = 200;
        public const int invalid_request = 400;
        public const int invalid_token = 401;
        public const int unauthorized_client = 400;
        public const int access_denied = 400;
        public const int unsupported_response_type = 400;
        public const int invalid_scope = 400;
        public const int server_error = 400;
        public const int temporarily_unavailable = 400;
        public const int bad_request = 400;
        public const int invalid_client = 401;
        public const int unauthorized = 401;
        public const int invalid_grant = 400;
        public const int token_expired = 401;
        public const int invalid_callback = 401;
        public const int invalid_client_secret = 401;
        public const int unsupported_grant_type = 400;
        public const int unsupported_token_type = 400;
        public const int insufficient_Scope = 403;
        public const int not_found = 404;

        public static int GetHttpStatusCode(string openIdErrorCode)
        {
            var errorStatusCode = openIdErrorCode switch
            {
                "invalid_request" => invalid_request,
                "invalid_token" => invalid_token,
                "unauthorized_client" => unauthorized_client,
                "access_denied" => access_denied,
                "unsupported_response_type" => unsupported_response_type,
                "invalid_scope" => invalid_scope,
                "server_error" => server_error,
                "temporarily_unavailable" => temporarily_unavailable,
                "invalid_client" => invalid_client,
                "invalid_grant" => invalid_grant,
                "token_expired" => token_expired,
                "invalid_callback" => invalid_callback,
                "invalid_client_secret" => invalid_client_secret,
                "unsupported_grant_type" => unsupported_grant_type,
                "unsupported_token_type" => unsupported_token_type,
                "insufficient_scope" => insufficient_Scope,
                "sucess" => success,
                _ => invalid_request
            };
            return errorStatusCode;
        }
    }

    public static class GrantTypes
    {
        public const string Password = "password";
        public const string AuthorizationCode = "authorization_code";
        public const string ClientCredentials = "client_credentials";

        public const string RefreshToken = "refresh_token";
        /// <summary>Exchange external sign-in verification code (e.g. after Google) for tokens.</summary>
        public const string UserCode = "user_code";
        //public const string Saml2Bearer = "urn:ietf:params:oauth:grant-type:saml2-bearer";
        //public const string JwtBearer = "urn:ietf:params:oauth:grant-type:jwt-bearer";
        //public const string TokenExchange = "urn:ietf:params:oauth:grant-type:token-exchange";
    }

    public static class TokenResponseType
    {
        public const string AccessToken = "access_token";
        public const string ExpiresIn = "expires_in";
        public const string TokenType = "token_type";
        public const string RefreshToken = "refresh_token";
        public const string IdentityToken = "id_token";
        public const string Error = "error";
        public const string ErrorDescription = "error_description";
        public const string BearerTokenType = "Bearer";
        public const string IssuedTokenType = "issued_token_type";
        public const string Scope = "scope";
    }

    public static class ResponseModes
    {
        public const string FormPost = "form_post";
        public const string Query = "query";
        public const string Fragment = "fragment";
    }

    public static class ResponseTypes
    {
        public const string Code = "code";
        public const string Token = "token";
        public const string IdToken = "id_token";
        public const string IdTokenToken = "id_token token";
        public const string CodeIdToken = "code id_token";
        public const string CodeToken = "code token";
        public const string CodeIdTokenToken = "code id_token token";
    }

    public static class TokenType
    {
        public const string AccessToken = "access_token";
        public const string IdentityToken = "id_token";
        public const string RefreshToken = "refresh_token";
        public const string AuthorizationCode = "authorization_code";
        public const string VerificationCode = "verification_code";
        public const string InitialAccessToken = "initial_access_token";
        public const string RequestParameter = "request_parameter";
        public const string LogoutToken = "logout_token";
    }

    public static class ClientAssertionTypes
    {
        public const string JwtBearer = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";
    }

    public static class CodeChallengeMethods
    {
        public const string Plain = "plain";
        public const string Sha256 = "S256";
    }

    public static class UserAuthenticationMethods
    {
        public const string Password = "pwd";
        public const string MultiFactorAuthentication = "mfa";
        public const string OneTimePassword = "otp";
    }

    public static class AuthorizeRequest
    {
        public const string Scope = "scope";
        public const string CodeChallengeMethod = "code_challenge_method";

        public const string CodeChallenge = "code_challenge";

        //public const string AcrValues = "acr_values";
        //public const string LoginHint = "login_hint";
        //public const string IdTokenHint = "id_token_hint";
        //public const string UiLocales = "ui_locales";
        public const string MaxAge = "max_age";

        public const string Prompt = "prompt";

        //public const string Display = "display";
        public const string Nonce = "nonce";
        public const string ResponseMode = "response_mode";
        public const string State = "state";
        public const string RedirectUri = "redirect_uri";
        public const string ClientId = "client_id";
        public const string ResponseType = "response_type";
        public const string Request = "request";
        public const string RequestUri = "request_uri";
        public const string Audience = "aud";
    }

    public static class IntrospectionRequest
    {
        public const string Token = "token";
        public const string Scope = "scope";
        public const string TokenHintType = "token_hint_type";
    }

    public static class TokenHintTypes
    {
        public const string AccessToken = "access_token";
        public const string RefreshToken = "refresh_token";
    }

    public static class TokenRequest
    {
        public const string GrantType = "grant_type";
        public const string RedirectUri = "redirect_uri";
        public const string ClientId = "client_id";
        public const string ClientSecret = "client_secret";
        public const string ClientAssertion = "client_assertion";
        public const string ClientAssertionType = "client_assertion_type";
        public const string Assertion = "assertion";
        public const string Code = "code";
        public const string RefreshToken = "refresh_token";
        public const string Scope = "scope";
        public const string UserName = "username";
        public const string UserId = "userid";
        public const string Password = "password";
        public const string CodeVerifier = "code_verifier";
        public const string TokenType = "token_type";
        public const string Algorithm = "alg";
        public const string Key = "key";
        public const string Token = "token";
        /// <summary>Verification code from external sign-in redirect (e.g. UserCode query param).</summary>
        public const string UserCode = "user_code";

        //public const string DeviceCode = "device_code";

        // token exchange
        public const string Resource = "resource";

        public const string Audience = "audience";
        //public const string RequestedTokenType = "requested_token_type";
        //public const string SubjectToken = "subject_token";
        //public const string SubjectTokenType = "subject_token_type";
        //public const string ActorToken = "actor_token";
        //public const string ActorTokenType = "actor_token_type";
    }

    public static class PromptModes
    {
        public const string None = "none";

        public const string Login = "login";

        //public const string Consent = "consent";
        public const string SelectAccount = "select_account";
    }

    public static class AuthenticationSchemes
    {
        public const string AuthorizationHeaderBearer = "Bearer";
        //public const string FormPostBearer = "access_token";
        //public const string QueryStringBearer = "access_token";
        //public const string AuthorizationHeaderPop = "PoP";
        //public const string FormPostPop = "pop_access_token";
        //public const string QueryStringPop = "pop_access_token";
    }

    public static class RoutePathParameters
    {
        public const string Error = "errorId";
        public const string Login = "returnUrl";
        public const string Consent = "returnUrl";
        public const string Logout = "logoutId";
        public const string EndSessionCallback = "endSessionId";
        public const string Custom = "returnUrl";
        public const string UserCode = "userCode";
    }

    public static class Algorithms
    {
        public const string EcdsaSha256 = "ES256";

        public const string EcdsaSha384 = "ES384";

        public const string EcdsaSha512 = "ES512";
        public const string HmacSha384 = "HS384";
        public const string HmacSha512 = "HS512";
        public const string HmacSha256 = "HS256";

        public const string RsaSha256 = "RS256";
        public const string RsaSha384 = "RS384";
        public const string RsaSha512 = "RS512";
        public const string RsaSsaPssSha256 = "PS256";
        public const string RsaSsaPssSha384 = "PS384";
        public const string RsaSsaPssSha512 = "PS512";
    }

    public static class LogoutTokenEvents
    {
        public const string BackChannelLogoutUri = "http://schemas.openid.net/event/backchannel-logout";
        public const string LogoutToken = "logout_token";
    }

    //public static class ResponseTypes
    //{
    //    public const string AuthorizationCode = "code";
    //    public const string IdentityToken = "id_token";
    //    public const string AccessToken = "token";
    //}
}
