/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain.Configurations.Api;
using HCL.CS.Service.Interfaces.Interfaces.Api;

namespace HCL.CS.Service.Implementation.Api.Ldap;

public sealed class LdapConfigurationValidator : ILdapConfigurationValidator
{
    public IReadOnlyList<string> Validate(LdapConfig configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        if (!configuration.IsEnabled) return Array.Empty<string>();

        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(configuration.LdapHostName))
            errors.Add("LDAP host is required when LDAP authentication is enabled.");
        if (configuration.LdapPort is <= 0 or > 65535)
            errors.Add("LDAP port must be between 1 and 65535.");
        if (string.IsNullOrWhiteSpace(configuration.LdapDomainName))
            errors.Add("LDAP base DN is required.");
        if (string.IsNullOrWhiteSpace(configuration.UserSearchFilter) ||
            !configuration.UserSearchFilter.Contains("{0}", StringComparison.Ordinal))
            errors.Add("LDAP user search filter must contain the escaped identifier placeholder '{0}'.");
        if (configuration.ConnectTimeoutSeconds is < 1 or > 120)
            errors.Add("LDAP connect timeout must be between 1 and 120 seconds.");
        if (configuration.SearchTimeoutSeconds is < 1 or > 120)
            errors.Add("LDAP search timeout must be between 1 and 120 seconds.");
        if (configuration.IsSslEnabled && configuration.UseStartTls)
            errors.Add("LDAP SSL and StartTLS cannot both be enabled.");
        if (!configuration.IsSslEnabled && !configuration.UseStartTls &&
            !IsAllowedUnencryptedDevelopmentEndpoint(configuration))
            errors.Add("Encrypted LDAP transport is required.");
        if (string.IsNullOrWhiteSpace(configuration.BindDn) !=
            string.IsNullOrWhiteSpace(configuration.BindPassword))
            errors.Add("LDAP bind DN and bind password must either both be configured or both be empty.");

        ValidateAttributeName(configuration.Attributes?.ImmutableId, "immutable ID", errors);
        ValidateAttributeName(configuration.Attributes?.Email, "email", errors);
        ValidateAttributeName(configuration.Attributes?.DisplayName, "display name", errors);
        ValidateAttributeName(configuration.Attributes?.AccountStatus, "account status", errors);
        if (configuration.RequireEmployeeId)
            ValidateAttributeName(configuration.Attributes?.EmployeeId, "employee ID", errors);
        if (configuration.RequireDepartment)
            ValidateAttributeName(configuration.Attributes?.Department, "department", errors);
        if (configuration.RequireUserPrincipalName)
            ValidateAttributeName(configuration.Attributes?.UserPrincipalName, "user principal name", errors);

        return errors;
    }

    private static bool IsAllowedUnencryptedDevelopmentEndpoint(LdapConfig configuration)
    {
        if (!configuration.AllowUnencryptedForDevelopment) return false;

        return configuration.LdapHostName.Equals("localhost", StringComparison.OrdinalIgnoreCase)
               || configuration.LdapHostName.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase)
               || configuration.LdapHostName.Equals("::1", StringComparison.OrdinalIgnoreCase);
    }

    private static void ValidateAttributeName(string? attributeName, string displayName, ICollection<string> errors)
    {
        if (string.IsNullOrWhiteSpace(attributeName))
            errors.Add($"LDAP {displayName} attribute mapping is required.");
    }
}
