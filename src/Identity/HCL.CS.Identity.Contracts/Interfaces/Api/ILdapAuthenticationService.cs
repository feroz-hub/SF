/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain.Configurations.Api;
using HCL.CS.Domain.Models.Api;

namespace HCL.CS.Service.Interfaces.Interfaces.Api;

public interface ILdapAuthenticationService
{
    Task<LdapAuthenticationResult> AuthenticateAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default);
}

public interface ILdapAccountValidationService
{
    Task<LdapAccountValidationResult> ValidateAccountAsync(
        string directoryImmutableId,
        CancellationToken cancellationToken = default);
}

public interface ILdapProtocolClient
{
    Task<IReadOnlyList<LdapDirectoryEntry>> SearchUsersAsync(
        string escapedIdentifier,
        CancellationToken cancellationToken = default);

    Task ValidateCredentialsAsync(
        string distinguishedName,
        string password,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LdapDirectoryEntry>> SearchByImmutableIdAsync(
        string directoryImmutableId,
        CancellationToken cancellationToken = default);
}

public interface ILdapAttributeMapper
{
    LdapUserProfile Map(LdapDirectoryEntry entry);
}

public interface ILdapAccountStatusEvaluator
{
    LdapAccountState Evaluate(object? rawStatus);
}

public interface ILdapConfigurationValidator
{
    IReadOnlyList<string> Validate(LdapConfig configuration);
}

public interface ILdapAuthenticationAuditLogger
{
    void AuthenticationSucceeded(string pseudonymousUserIdentifier);

    void AuthenticationFailed(string failureCode, string pseudonymousUserIdentifier);
}
