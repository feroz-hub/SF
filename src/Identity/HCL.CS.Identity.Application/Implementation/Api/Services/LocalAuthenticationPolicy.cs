/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Globalization;
using System.Net.Mail;
using HCL.CS.Domain;
using HCL.CS.Domain.Configurations.Api;
using HCL.CS.Domain.Enums;
using HCL.CS.Domain.Models.Api;
using HCL.CS.Service.Interfaces.Interfaces.Api;

namespace HCL.CS.Service.Implementation.Api.Services;

public sealed class AuthenticationModeResolver(HclCsConfig configuration) : IAuthenticationModeResolver
{
    private readonly LdapConfig ldap = configuration.SystemSettings.LdapConfig;

    public AuthenticationMode Resolve()
    {
        return ldap.Enabled == true && HasMandatoryLdapConfiguration()
            ? AuthenticationMode.Ldap
            : AuthenticationMode.Local;
    }

    public bool HasMandatoryLdapConfiguration()
    {
        return !string.IsNullOrWhiteSpace(ldap.LdapHostName)
               && ldap.LdapPort is > 0 and <= 65535
               && !string.IsNullOrWhiteSpace(ldap.LdapDomainName)
               && !string.IsNullOrWhiteSpace(ldap.UserSearchFilter)
               && ldap.UserSearchFilter.Contains("{0}", StringComparison.Ordinal)
               && !string.IsNullOrWhiteSpace(ldap.Attributes?.ImmutableId)
               && !string.IsNullOrWhiteSpace(ldap.Attributes?.Email)
               && !string.IsNullOrWhiteSpace(ldap.Attributes?.DisplayName)
               && !string.IsNullOrWhiteSpace(ldap.Attributes?.AccountStatus);
    }
}

public sealed class AllowedEmailDomainPolicy(HclCsConfig configuration) : IAllowedEmailDomainPolicy
{
    public EmailDomainValidationResult Validate(string? email)
    {
        const string invalidCode = "AUTH_LOCAL_EMAIL_INVALID";
        if (string.IsNullOrWhiteSpace(email))
            return Invalid(invalidCode);

        var trimmed = email.Trim();
        if (!MailAddress.TryCreate(trimmed, out var address)
            || !string.Equals(address.Address, trimmed, StringComparison.OrdinalIgnoreCase)
            || address.Address.Count(character => character == '@') != 1)
            return Invalid(invalidCode);

        var separator = address.Address.LastIndexOf('@');
        if (separator <= 0 || separator == address.Address.Length - 1)
            return Invalid(invalidCode);

        var localPart = address.Address[..separator];
        var domain = address.Address[(separator + 1)..].TrimEnd('.').ToLowerInvariant();
        if (localPart.Length > 64 || address.Address.Length > 254 || domain.Any(character => character > 127))
            return Invalid(invalidCode);

        var allowedDomains = configuration.SystemSettings.LocalAuthenticationConfig.AllowedEmailDomains;
        var allowed = allowedDomains.Any(candidate =>
            string.Equals(candidate?.Trim().TrimEnd('.'), domain, StringComparison.OrdinalIgnoreCase));
        if (!allowed)
            return new EmailDomainValidationResult
            {
                IsValid = false,
                NormalizedDomain = domain,
                ReasonCode = "AUTH_LOCAL_EMAIL_DOMAIN_NOT_ALLOWED"
            };

        return new EmailDomainValidationResult
        {
            IsValid = true,
            NormalizedEmail = string.Create(
                CultureInfo.InvariantCulture,
                $"{localPart.ToLowerInvariant()}@{domain}"),
            NormalizedDomain = domain
        };
    }

    private static EmailDomainValidationResult Invalid(string reasonCode)
    {
        return new EmailDomainValidationResult
        {
            IsValid = false,
            ReasonCode = reasonCode
        };
    }
}

public sealed class LocalAuthenticationConfigurationValidator : ILocalAuthenticationConfigurationValidator
{
    public IReadOnlyList<string> Validate(LocalAuthenticationConfig configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var errors = new List<string>();
        if (!configuration.EnabledWhenLdapDisabledOrUnconfigured)
            errors.Add("Local authentication must be enabled when LDAP is disabled or unconfigured.");
        if (!configuration.RequireEmailConfirmation)
            errors.Add("Local authentication must require email confirmation.");
        if (!configuration.AllowSelfRegistration && !configuration.AllowAdministratorCreation)
            errors.Add("At least one local user creation path must be enabled.");

        var domains = configuration.AllowedEmailDomains?
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim().ToLowerInvariant())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray() ?? [];
        if (domains.Length != 1 || domains[0] != "hcltech.com")
            errors.Add("The local authentication domain must be exactly 'hcltech.com'.");
        if (domains.Any(domain =>
                domain.Contains('@')
                || domain.Contains('*')
                || domain.Contains("://", StringComparison.Ordinal)
                || domain.StartsWith('.')
                || domain.EndsWith('.')))
            errors.Add("Local authentication domains must be plain exact DNS domains.");

        return errors;
    }
}
