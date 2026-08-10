/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Net.Mail;
using HCL.CS.Domain;
using HCL.CS.Domain.Configurations.Api;
using HCL.CS.Domain.Models.Api;
using HCL.CS.Service.Interfaces.Interfaces.Api;

namespace HCL.CS.Service.Implementation.Api.Ldap;

public sealed class LdapAttributeMapper(
    HclCsConfig configuration,
    ILdapAccountStatusEvaluator accountStatusEvaluator)
    : ILdapAttributeMapper
{
    private const int MaxImmutableIdLength = 512;
    private const int MaxIdentityTextLength = 255;
    private const int MaxEmailLength = 320;
    private readonly LdapConfig ldapConfig = configuration.SystemSettings.LdapConfig;

    public LdapUserProfile Map(LdapDirectoryEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        var attributes = ldapConfig.Attributes;
        var immutableId = ReadImmutableId(entry, attributes.ImmutableId);
        var email = ReadRequiredText(entry, attributes.Email, MaxEmailLength).ToLowerInvariant();
        var displayName = ReadRequiredText(entry, attributes.DisplayName, MaxIdentityTextLength);
        var employeeId = ReadOptionalText(entry, attributes.EmployeeId, MaxIdentityTextLength);
        var department = ReadOptionalText(entry, attributes.Department, MaxIdentityTextLength);
        var userPrincipalName = ReadOptionalText(entry, attributes.UserPrincipalName, MaxIdentityTextLength)?
            .ToLowerInvariant() ?? string.Empty;
        var rawStatus = ReadFirstValue(entry, attributes.AccountStatus);
        var accountState = accountStatusEvaluator.Evaluate(rawStatus);

        if (!IsValidEmail(email)) throw new LdapProfileValidationException(LdapFailureCodes.EmailInvalid);
        if (ldapConfig.RequireEmployeeId && string.IsNullOrWhiteSpace(employeeId))
            throw new LdapProfileValidationException(LdapFailureCodes.RequiredAttributeMissing);
        if (ldapConfig.RequireDepartment && string.IsNullOrWhiteSpace(department))
            throw new LdapProfileValidationException(LdapFailureCodes.RequiredAttributeMissing);
        if (ldapConfig.RequireUserPrincipalName && string.IsNullOrWhiteSpace(userPrincipalName))
            throw new LdapProfileValidationException(LdapFailureCodes.RequiredAttributeMissing);
        if (accountState == LdapAccountState.Unknown)
            throw new LdapProfileValidationException(LdapFailureCodes.SchemaMappingFailed);

        return new LdapUserProfile
        {
            ImmutableId = immutableId,
            EmployeeId = employeeId,
            UserPrincipalName = userPrincipalName,
            Email = email,
            DisplayName = displayName,
            Department = department,
            AccountStatus = accountState.ToString().ToUpperInvariant()
        };
    }

    private static string ReadImmutableId(LdapDirectoryEntry entry, string attributeName)
    {
        var value = ReadFirstValue(entry, attributeName);
        if (value is null) throw new LdapProfileValidationException(LdapFailureCodes.RequiredAttributeMissing);

        string immutableId;
        if (value is byte[] bytes)
        {
            if (bytes.Length != 16)
                throw new LdapProfileValidationException(LdapFailureCodes.ImmutableIdInvalid);
            immutableId = new Guid(bytes).ToString("D");
        }
        else
        {
            immutableId = Convert.ToString(value)?.Trim() ?? string.Empty;
        }

        if (string.IsNullOrWhiteSpace(immutableId) || immutableId.Length > MaxImmutableIdLength)
            throw new LdapProfileValidationException(LdapFailureCodes.ImmutableIdInvalid);

        return immutableId;
    }

    private static string ReadRequiredText(LdapDirectoryEntry entry, string attributeName, int maximumLength)
    {
        var value = ReadOptionalText(entry, attributeName, maximumLength);
        if (string.IsNullOrWhiteSpace(value))
            throw new LdapProfileValidationException(LdapFailureCodes.RequiredAttributeMissing);
        return value;
    }

    private static string? ReadOptionalText(LdapDirectoryEntry entry, string attributeName, int maximumLength)
    {
        var value = ReadFirstValue(entry, attributeName);
        if (value is null) return null;
        if (value is byte[]) throw new LdapProfileValidationException(LdapFailureCodes.SchemaMappingFailed);

        var normalized = Convert.ToString(value)?.Trim();
        if (string.IsNullOrEmpty(normalized)) return null;
        if (normalized.Length > maximumLength)
            throw new LdapProfileValidationException(LdapFailureCodes.SchemaMappingFailed);
        return normalized;
    }

    private static object? ReadFirstValue(LdapDirectoryEntry entry, string attributeName)
    {
        if (string.IsNullOrWhiteSpace(attributeName) ||
            !entry.Attributes.TryGetValue(attributeName, out var values) ||
            values is null)
            return null;

        return values.FirstOrDefault(value => value is not null);
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var address = new MailAddress(email);
            return address.Address.Equals(email, StringComparison.OrdinalIgnoreCase);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
