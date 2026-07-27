/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain.Configurations.Api;
using HCL.CS.Domain.Enums;
using HCL.CS.Domain.Models.Api;

namespace HCL.CS.Service.Interfaces.Interfaces.Api;

public interface IAuthenticationModeResolver
{
    AuthenticationMode Resolve();

    bool HasMandatoryLdapConfiguration();
}

public interface IAllowedEmailDomainPolicy
{
    EmailDomainValidationResult Validate(string? email);
}

public interface ILocalAuthenticationConfigurationValidator
{
    IReadOnlyList<string> Validate(LocalAuthenticationConfig configuration);
}
