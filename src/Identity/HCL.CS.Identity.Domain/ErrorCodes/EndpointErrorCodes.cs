namespace HCL.CS.Domain.ErrorCodes;

public static class EndpointErrorCodes
{
    // Common
    public const string NoRecordsFound = "NO_RECORDS_FOUND";

    public const string ClientNameIsRequired = "CLIENT_NAME_REQUIRED";
    public const string ClientSecretExpired = "CLIENT_SECRET_EXPIRED";
    public const string ClientSecretInvalid = "CLIENT_SECRET_INVALID";
    public const string ClientNameAlreadyExists = "CLIENT_NAME_ALREADY_EXISTS";
    public const string InactiveClient = "INACTIVE_CLIENT";
    public const string ClientDoesNotExist = "CLIENT_DOES_NOT_EXIST";
    public const string ClientSecretExpiresAtRequired = "CLIENTSECRET_EXPIRES_AT_REQUIRED";
    public const string InvalidClientObject = "INVALID_CLIENT_OBJECT";
    public const string NoClientRecordsFound = "NO_CLIENT_RECORDS_FOUND";
    public const string ClientIdMissingInSecret = "CLIENTID_MISSING_IN_SECRET";
    public const string ClientIdMissingInRequest = "CLIENTID_MISSING_IN_REQUEST";
    public const string ClientIdTooLong = "CLIENT_ID_TOO_LONG";
    public const string ClientNameTooLong = "CLIENT_NAME_TOO_LONG";
    public const string ClientNotAuthorizedForGrantType = "CLIENT_NOTAUTHORIZED_GRANT_TYPE";
    public const string InvalidClientBinding = "INVALID_CLIENT_BINDING";
    public const string ClientSecretNotAvailable = "CLIENT_SECRET_NOTAVAILABLE";
    public const string AllowedScopesIsRequired = "ALLOWED_SCOPES_REQUIRED";
    public const string SupportedGrantTypesIsRequired = "SUPPORTED_GRANT_TYPES_REQUIRED";
    public const string SupportedResponseTypesIsRequired = "SUPPORTED_RESPONSE_TYPES_REQUIRED";
    public const string ClientIdIsRequired = "CLIENT_ID_REQUIRED";
    public const string ClientSecretIsRequired = "CLIENT_SECRET_REQUIRED";
    public const string InvalidAccessTokenExpireRange = "INVALID_ACCESS_TOKEN_EXPIRE_RANGE";
    public const string InvalidIdentityTokenExpireRange = "INVALID_IDENTITY_TOKEN_EXPIRE_RANGE";
    public const string InvalidRefreshTokenExpireRange = "INVALID_REFRESH_TOKEN_EXPIRE_RANGE";
    public const string InvalidLogoutTokenExpireRange = "INVALID_LOGOUT_TOKEN_EXPIRE_RANGE";
    public const string InvalidAuthorizationCodeExpireRange = "INVALID_AUTHORIZATION_CODE_EXPIRE_RANGE";
    public const string SigningAlgorithmIsInvalid = "SIGNING_ALGORITHM_IS_INVALID";
    public const string ModifiedByTooLong = "MODIFIEDBY_TOO_LONG";

    public const string UserNameMissing = "USERNAME_MISSING";
    public const string UserNameTooLong = "USERNAME_TOO_LONG";
    public const string PasswordTooLong = "PASSWORD_TOO_LONG";
    public const string UserAuthenticationFailed = "USER_AUTHENTICATION_FAILED";
    public const string InvalidSubjectId = "INVALID_SUBJECT_ID";
    public const string InvalidUserClaims = "INVALID_USER_CLAIMS";
    public const string UserDoesNotExist = "USER_DOESNOT_EXIST";
    public const string NoUserRoleMapped = "NO_USERROLE_MAPPED";
    public const string SubjectClaimValueMismatch = "SUBJECT_CLAIM_VALUE_MISMATCH";
    public const string TwoFactorEnabled = "TWO_FACTOR_ENABLED";
    public const string InvalidUser = "INVALID_USER";
    public const string NoUserRoleClaimMapped = "NO_USERROLE_CLAIM_MAPPED";

    public const string InvalidLogoUri = "INVALID_LOGO_URI";
    public const string InvalidClientUri = "INVALID_CLIENT_URI";
    public const string InvalidPolicyUri = "INVALID_POLICY_URI";
    public const string InvalidRedirectUri = "INVALID_REDIRECT_URI";
    public const string RedirectUriNotRegistered = "REDIRECT_URI_NOT_REGISTERED";
    public const string RedirectUriTooLong = "REDIRECT_URI_TOO_LONG";
    public const string RedirectURIIsMandatory = "REDIRECT_URI_IS_MANDATORY";
    public const string InvalidPostLogoutRedirectUri = "INVALID_POSTLOGOUT_REDIRECT_URI";
    public const string PostRedirectUriTooLong = "POST_REDIRECT_URI_TOO_LONG";
    public const string RedirectUriMissing = "REDIRECT_URI_MISSING";
    public const string InvalidTermsOfService = "INVALID_TERMSOFSERVICE_URI";
    public const string RedirectUriCreatedByTooLong = "REDIRECTURI_CREATEDBY_TOO_LONG";
    public const string PostRedirectUriCreatedByTooLong = "POSTREDIRECTURI_CREATEDBY_TOO_LONG";
    public const string RedirectUriModifiedByTooLong = "REDIRECTURI_MODIFIEDBY_TOO_LONG";
    public const string PostRedirectUriModifiedByTooLong = "POSTREDIRECTURI_MODIFIEDBY_TOO_LONG";
    public const string InvalidRedirectUriClientId = "INVALID_REDIRECTURI_CLIENTID";
    public const string InvalidPostRedirectUriClientId = "INVALID_POST_REDIRECTURI_CLIENTID";
    public const string InactiveClientRedirectUri = "INACTIVE_CLIENT_REDIRECTURI";
    public const string InactiveClientPostLogoutRedirectUri = "INACTIVE_CLIENT_POSTLOGOUTREDIRECTURI";

    public const string TokenMissing = "TOKEN_MISSING";
    public const string Invalid_Token_Length = "INVALID_TOKEN_LENGTH";
    public const string InvalidTokenFormat = "INVALID_TOKEN_FORMAT";
    public const string ClientNotConfiguredToReceiveAccessToken = "CLIENT_NOT_CONFIGURED_FOR_ACCESSTOKEN";
    public const string InvalidTokenInIntrospection = "INVALID_TOKEN_IN_INTROSPECTION";
    public const string TokenExpired = "TOKEN_EXPIRED";
    public const string TokenRevoked = "TOKEN_REVOKED";
    public const string InvalidTokenHintType = "INVALID_TOKEN_HINT_TYPE";
    public const string UnsupportedTokenType = "UNSUPPORTED_TOKEN_TYPE";
    public const string NoTokenFound = "NO_TOKEN_FOUND";
    public const string RefreshTokenRequestNotAllowed = "REFRESH_TOKEN_REQUEST_NOT_ALLOWED";
    public const string RefreshTokenMissing = "REFRESH_TOKEN_MISSING";
    public const string RefreshTokenTooLong = "REFRESH_TOKEN_TOO_LONG";
    public const string RefreshTokenValidationFailed = "REFRESH_TOKEN_VALIDATION_FAILED";
    public const string TokenMaxLengthExceeded = "TOKEN_MAX_LENGTH_EXCEEDED";
    public const string RefreshTokenDoesNotExist = "REFRESHTOKEN_DOESNOT_EXIST";
    public const string TokenIsNullOrInvalid = "TOKEN_NULL_OR_INVALID";
    public const string FailedToStoreRefreshToken = "FAILED_TO_STORE_REFRESHTOKEN";
    public const string FailedToStoreReturnUrl = "FAILED_TO_STORE_RETURNURL";
    public const string BackChannelRequiredSessionId = "BACK_CHANNEL_REQUIRED_SESSIONID";

    public const string ResponseTypeMissing = "RESPONSE_TYPE_MISSING";
    public const string InvalidResponseMode = "INVALID_RESPONSE_MODE";
    public const string InvalidGrantTypeForEndpoint = "INVALID_GRANTTYPE_FOR_ENDPOINT";
    public const string InvalidGrantTypeForClient = "INVALID_GRANTTYPE_FOR_CLIENT";
    public const string GrantTypeIsMissing = "GRANT_TYPE_MISSING";
    public const string GrantTypeTooLong = "GRANT_TYPE_TOO_LONG";

    public const string ScopeMissing = "SCOPE_MISSING";
    public const string ScopeTooLong = "SCOPE_TOO_LONG";
    public const string InvalidScopeOrNotAllowed = "INVALIDSCOPE_OR_NOT_ALLOWED";
    public const string ResponseTypeRequiresOpenIdScope = "REQUIRES_OPEN_ID_SCOPE";
    public const string RequestedScopeNotAllowedForClient = "REQUEST_SCOPE_NOT_ALLOWED";
    public const string OpenIdScopeMissing = "OPEN_ID_SCOPE_MISSING";
    public const string MustIncludeIdentityScopes = "MUST_INCLUDE_IDENTITY_SCOPES";
    public const string MustNotIncludeResourceScopes = "MUST_NOT_INCLUDE_RESOURCE_SCOPES";
    public const string MustNotIncludeOfflineAccessScope = "MUST_NOT_INCLUDE_OFFLINE_ACCESS_SCOPE";
    public const string MustIncludeResourceScopes = "MUST_INCLUDE_RESOURCE_SCOPES";
    public const string OpenIdScopeNotAllowed = "OPEN_ID_SCOPE_NOT_ALLOWED";
    public const string InvalidScopeClaims = "INVALID_SCOPE_CLAIMS";

    public const string InvalidCodeChallenge = "INVALID_CODE_CHALLENGE";
    public const string ClientMissingCodeChallenge = "CLIENT_MISSING_CODE_CHALLENGE";
    public const string ClientMissingCodeChallengeMethod = "CLIENT_MISSING_CODE_CHALLENGE_METHOD";
    public const string CodeVerifierMissing = "CODE_VERIFIER_MISSING";
    public const string CodeVerifierTooShort = "CODE_VERIFIER_TOO_SHORT";
    public const string CodeVerifierTooLong = "CODE_VERIFIER_TOO_LONG";
    public const string UnsupportedCodeChallengeMethod = "UNSUPPORTED_CODE_CHALLENGE_METHOD";

    public const string JwtRequestUriNotSupported = "JWT_REQUEST_URI_NOT_SUPPORTED";
    public const string InvalidNonce = "INVALID_NONCE";
    public const string InvalidPrompt = "INVALID_PROMPT";
    public const string InvalidMaxAge = "INVALID_MAX_AGE";

    public const string UnauthenticatedUser = "UNAUTHENTICATED_USER";
    public const string InvalidIdentityToken = "INVALID_IDENTITY_TOKEN";
    public const string InvalidState = "INVALID_STATE";

    public const string AuthorizationCodeMissing = "AUTH_CODE_MISSING";
    public const string AuthorizationCodeTooLong = "AUTH_CODE_TOO_LONG";
    public const string InvalidAuthorizationCode = "INVALID_AUTH_CODE";
    public const string AuthorizationCodeExpired = "AUTH_CODE_EXPIRED";
    public const string AuthorizationCodeScopeError = "AUTH_CODE_SCOPE_ERROR";
    public const string InvalidCodeVerifier = "INVALID_CODE_VERIFIER";

    public const string ArgumentNullError = "ARGUMENT_NULL_ERROR";
    public const string InvalidRequest = "INVALID_REQUEST";


    public const string InvalidSecurityTokenId = "INVALID_SECURITY_TOKEN_ID";

    public const string InvalidClientSecret = "INVALID_CLIENT_SECRET";

    public const string InvalidRevocationRequest = "INVALID_REVOCATIONREQUEST";
    public const string InvalidOperation = "INVALID_OPERATION";
    public const string ClientIdMaxLengthExceeded = "CLIENTID_MAX_LENGTH_EXCEEDED";
    public const string ClientSecretMaxLengthExceeded = "CLIENTSECRET_MAX_LENGTH_EXCEEDED";
    public const string HeaderContentIsNotProper = "HEADER_CONTENT_NOTPROPER";
    public const string InvalidHttpRequest = "INVALID_HTTP_REQUEST";
    public const string LoginRequiredAndPromptNone = "LOGINREQUIRED_AND_PROMPT_NONE";
}
