/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain;
using HCL.CS.Domain.Models.Api;
using HCL.CS.Service.Interfaces.Interfaces.Api;

namespace HCL.CS.Service.Implementation.Api.Ldap;

public sealed class LdapAccountValidationService(
    HclCsConfig configuration,
    ILdapConfigurationValidator configurationValidator,
    ILdapProtocolClient protocolClient,
    ILdapAttributeMapper attributeMapper)
    : ILdapAccountValidationService
{
    public async Task<LdapAccountValidationResult> ValidateAccountAsync(
        string directoryImmutableId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(directoryImmutableId))
            return LdapAccountValidationResult.Failed(LdapFailureCodes.AccountRevalidationFailed);

        var ldapConfiguration = configuration.SystemSettings.LdapConfig;
        if (!ldapConfiguration.IsEnabled ||
            configurationValidator.Validate(ldapConfiguration).Count > 0)
            return LdapAccountValidationResult.Failed(LdapFailureCodes.ConfigurationInvalid);

        try
        {
            var matches = await protocolClient.SearchByImmutableIdAsync(
                directoryImmutableId,
                cancellationToken);
            if (matches.Count == 0)
                return LdapAccountValidationResult.Failed(LdapFailureCodes.UserNotFound);
            if (matches.Count > 1)
                return LdapAccountValidationResult.Failed(LdapFailureCodes.MultipleUsersFound);

            var profile = attributeMapper.Map(matches[0]);
            if (!string.Equals(
                    profile.ImmutableId,
                    directoryImmutableId,
                    StringComparison.OrdinalIgnoreCase))
                return LdapAccountValidationResult.Failed(LdapFailureCodes.SchemaMappingFailed);

            var state = Enum.TryParse<LdapAccountState>(
                profile.AccountStatus,
                true,
                out var parsedState)
                ? parsedState
                : LdapAccountState.Unknown;
            return state == LdapAccountState.Active
                ? LdapAccountValidationResult.Active()
                : LdapAccountValidationResult.Failed(GetFailureCode(state), state);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (LdapProfileValidationException exception)
        {
            return LdapAccountValidationResult.Failed(exception.FailureCode);
        }
        catch (LdapProtocolException exception)
        {
            return LdapAccountValidationResult.Failed(exception.FailureCode);
        }
        catch
        {
            return LdapAccountValidationResult.Failed(LdapFailureCodes.UnknownFailure);
        }
    }

    private static string GetFailureCode(LdapAccountState state)
    {
        return state switch
        {
            LdapAccountState.Disabled => LdapFailureCodes.AccountDisabled,
            LdapAccountState.Locked => LdapFailureCodes.AccountLocked,
            LdapAccountState.Expired => LdapFailureCodes.AccountExpired,
            _ => LdapFailureCodes.AccountInactive
        };
    }
}
