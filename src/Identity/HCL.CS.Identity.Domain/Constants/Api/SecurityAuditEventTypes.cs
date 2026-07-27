/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

namespace HCL.CS.Domain.Constants.Api;

public static class SecurityAuditEventTypes
{
    public const string AuthenticationModeResolvedLdap = "AUTH_MODE_RESOLVED_LDAP";
    public const string AuthenticationModeResolvedLocal = "AUTH_MODE_RESOLVED_LOCAL";
    public const string AuthenticationSucceeded = "AUTHENTICATION_SUCCEEDED";
    public const string AuthenticationFailed = "AUTHENTICATION_FAILED";
    public const string LdapAuthenticationSucceeded = "LDAP_AUTHENTICATION_SUCCEEDED";
    public const string LdapAuthenticationFailed = "LDAP_AUTHENTICATION_FAILED";
    public const string LocalAuthenticationSucceeded = "LOCAL_AUTHENTICATION_SUCCEEDED";
    public const string LocalAuthenticationFailed = "LOCAL_AUTHENTICATION_FAILED";
    public const string LocalRegistrationRequested = "AUTH_LOCAL_REGISTRATION_REQUESTED";
    public const string LocalRegistrationSucceeded = "AUTH_LOCAL_REGISTRATION_SUCCEEDED";
    public const string LocalRegistrationRejected = "AUTH_LOCAL_REGISTRATION_REJECTED";
    public const string LocalDomainRejected = "AUTH_LOCAL_DOMAIN_REJECTED";
    public const string LocalIdentityConflict = "AUTH_LOCAL_IDENTITY_CONFLICT";
    public const string LocalEmailConfirmed = "AUTH_LOCAL_EMAIL_CONFIRMED";
    public const string LocalEmailConfirmationRejected = "AUTH_LOCAL_EMAIL_CONFIRMATION_REJECTED";
    public const string LdapLocalFallbackBlocked = "AUTH_LDAP_LOCAL_FALLBACK_BLOCKED";
    public const string ExternalAuthenticationSucceeded = "EXTERNAL_AUTHENTICATION_SUCCEEDED";
    public const string ExternalAuthenticationFailed = "EXTERNAL_AUTHENTICATION_FAILED";
    public const string AuthorizationCodeIssued = "AUTHORIZATION_CODE_ISSUED";
    public const string TokenIssued = "TOKEN_ISSUED";
    public const string TokenRefreshSucceeded = "TOKEN_REFRESH_SUCCEEDED";
    public const string TokenRefreshRejected = "TOKEN_REFRESH_REJECTED";
    public const string RefreshTokenReuseDetected = "REFRESH_TOKEN_REUSE_DETECTED";
    public const string TokenRevoked = "TOKEN_REVOKED";
    public const string SessionLogout = "SESSION_LOGOUT";
    public const string LdapAccountRevalidationFailed = "LDAP_ACCOUNT_REVALIDATION_FAILED";
    public const string AmbiguousExternalIdentity = "AMBIGUOUS_EXTERNAL_IDENTITY";
}
