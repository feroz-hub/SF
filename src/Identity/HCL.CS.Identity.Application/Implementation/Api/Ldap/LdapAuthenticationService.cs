/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Security.Cryptography;
using System.Text;
using HCL.CS.Domain;
using HCL.CS.Domain.Models.Api;
using HCL.CS.Service.Interfaces.Interfaces.Api;

namespace HCL.CS.Service.Implementation.Api.Ldap;

public sealed class LdapAuthenticationService(
    HclCsConfig configuration,
    ILdapConfigurationValidator configurationValidator,
    ILdapProtocolClient protocolClient,
    ILdapAttributeMapper attributeMapper,
    ILdapAuthenticationAuditLogger auditLogger)
    : ILdapAuthenticationService
{
    public async Task<LdapAuthenticationResult> AuthenticateAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        var subjectHash = HashIdentifier(username);
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return Fail(LdapFailureCodes.InvalidCredentials, subjectHash);

        var ldapConfiguration = configuration.SystemSettings.LdapConfig;
        if (!ldapConfiguration.IsEnabled)
            return Fail(LdapFailureCodes.ConfigurationInvalid, subjectHash);

        var configurationErrors = configurationValidator.Validate(ldapConfiguration);
        if (configurationErrors.Count > 0)
            return Fail(LdapFailureCodes.ConfigurationInvalid, subjectHash);

        try
        {
            var escapedIdentifier = LdapFilterEncoder.Escape(username.Trim());
            var matches = await protocolClient.SearchUsersAsync(escapedIdentifier, cancellationToken);
            if (matches.Count == 0) return Fail(LdapFailureCodes.UserNotFound, subjectHash);
            if (matches.Count > 1) return Fail(LdapFailureCodes.MultipleUsersFound, subjectHash);

            var entry = matches[0];
            if (string.IsNullOrWhiteSpace(entry.DistinguishedName))
                return Fail(LdapFailureCodes.SchemaMappingFailed, subjectHash);

            await protocolClient.ValidateCredentialsAsync(
                entry.DistinguishedName,
                password,
                cancellationToken);
            var profile = attributeMapper.Map(entry);
            var accountFailure = GetAccountFailure(profile.AccountStatus);
            if (accountFailure is not null) return Fail(accountFailure, subjectHash);

            auditLogger.AuthenticationSucceeded(subjectHash);
            return LdapAuthenticationResult.Success(profile);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            auditLogger.AuthenticationFailed(LdapFailureCodes.Timeout, subjectHash);
            throw;
        }
        catch (LdapProfileValidationException exception)
        {
            return Fail(exception.FailureCode, subjectHash);
        }
        catch (LdapProtocolException exception)
        {
            return Fail(exception.FailureCode, subjectHash);
        }
        catch
        {
            return Fail(LdapFailureCodes.UnknownFailure, subjectHash);
        }
    }

    private LdapAuthenticationResult Fail(string failureCode, string subjectHash)
    {
        auditLogger.AuthenticationFailed(failureCode, subjectHash);
        return LdapAuthenticationResult.Failed(failureCode);
    }

    private static string? GetAccountFailure(string accountStatus)
    {
        return accountStatus switch
        {
            "ACTIVE" => null,
            "DISABLED" => LdapFailureCodes.AccountDisabled,
            "LOCKED" => LdapFailureCodes.AccountLocked,
            "EXPIRED" => LdapFailureCodes.AccountExpired,
            _ => LdapFailureCodes.AccountInactive
        };
    }

    private static string HashIdentifier(string? username)
    {
        var normalized = username?.Trim().ToLowerInvariant() ?? string.Empty;
        var digest = SHA256.HashData(Encoding.UTF8.GetBytes(normalized));
        return Convert.ToHexString(digest.AsSpan(0, 12));
    }
}
