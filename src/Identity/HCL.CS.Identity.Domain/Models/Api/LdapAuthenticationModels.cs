/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

namespace HCL.CS.Domain.Models.Api;

public sealed class LdapAuthenticationResult
{
    public bool IsAuthenticated { get; init; }

    public string? FailureCode { get; init; }

    public LdapUserProfile? User { get; init; }

    public static LdapAuthenticationResult Success(LdapUserProfile user)
    {
        ArgumentNullException.ThrowIfNull(user);
        return new LdapAuthenticationResult { IsAuthenticated = true, User = user };
    }

    public static LdapAuthenticationResult Failed(string failureCode)
    {
        return new LdapAuthenticationResult { FailureCode = failureCode };
    }
}

public sealed class LdapUserProfile
{
    public string ImmutableId { get; init; } = string.Empty;

    public string? EmployeeId { get; init; }

    public string UserPrincipalName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public string? Department { get; init; }

    public string AccountStatus { get; init; } = string.Empty;
}

public sealed class LdapAccountValidationResult
{
    public bool IsActive { get; init; }

    public LdapAccountState AccountState { get; init; } = LdapAccountState.Unknown;

    public string? FailureCode { get; init; }

    public static LdapAccountValidationResult Active()
    {
        return new LdapAccountValidationResult
        {
            IsActive = true,
            AccountState = LdapAccountState.Active
        };
    }

    public static LdapAccountValidationResult Failed(
        string failureCode,
        LdapAccountState accountState = LdapAccountState.Unknown)
    {
        return new LdapAccountValidationResult
        {
            FailureCode = failureCode,
            AccountState = accountState
        };
    }
}

public sealed class LdapDirectoryEntry
{
    public string DistinguishedName { get; init; } = string.Empty;

    public IReadOnlyDictionary<string, IReadOnlyList<object>> Attributes { get; init; } =
        new Dictionary<string, IReadOnlyList<object>>(StringComparer.OrdinalIgnoreCase);
}

public enum LdapAccountState
{
    Unknown = 0,
    Active = 1,
    Disabled = 2,
    Locked = 3,
    Expired = 4
}

public static class LdapFailureCodes
{
    public const string InvalidCredentials = "LDAP_INVALID_CREDENTIALS";
    public const string UserNotFound = "LDAP_USER_NOT_FOUND";
    public const string AccountDisabled = "LDAP_ACCOUNT_DISABLED";
    public const string AccountLocked = "LDAP_ACCOUNT_LOCKED";
    public const string AccountExpired = "LDAP_ACCOUNT_EXPIRED";
    public const string AccountInactive = "LDAP_ACCOUNT_INACTIVE";
    public const string RequiredAttributeMissing = "LDAP_REQUIRED_ATTRIBUTE_MISSING";
    public const string MultipleUsersFound = "LDAP_MULTIPLE_USERS_FOUND";
    public const string Timeout = "LDAP_TIMEOUT";
    public const string Unavailable = "LDAP_UNAVAILABLE";
    public const string TlsValidationFailed = "LDAP_TLS_VALIDATION_FAILED";
    public const string ConfigurationInvalid = "LDAP_CONFIGURATION_INVALID";
    public const string SchemaMappingFailed = "LDAP_SCHEMA_MAPPING_FAILED";
    public const string EmailInvalid = "LDAP_EMAIL_INVALID";
    public const string ImmutableIdInvalid = "LDAP_IMMUTABLE_ID_INVALID";
    public const string UnknownFailure = "LDAP_UNKNOWN_FAILURE";
    public const string AccountRevalidationFailed = "LDAP_ACCOUNT_REVALIDATION_FAILED";
}
