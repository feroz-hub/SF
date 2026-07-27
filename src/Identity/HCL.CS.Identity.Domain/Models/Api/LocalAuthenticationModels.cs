/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain.Enums;

namespace HCL.CS.Domain.Models.Api;

public sealed class EmailDomainValidationResult
{
    public bool IsValid { get; init; }

    public string? NormalizedEmail { get; init; }

    public string? NormalizedDomain { get; init; }

    public string? ReasonCode { get; init; }
}

public sealed class AuthenticationAvailabilityModel
{
    public string AuthenticationMode { get; init; } = string.Empty;

    public bool LocalRegistrationAvailable { get; init; }

    public string? AllowedEmailDomainHint { get; init; }

    public static AuthenticationAvailabilityModel Create(
        AuthenticationMode mode,
        bool localRegistrationAvailable)
    {
        return new AuthenticationAvailabilityModel
        {
            AuthenticationMode = mode == HCL.CS.Domain.Enums.AuthenticationMode.Ldap ? "LDAP" : "LOCAL",
            LocalRegistrationAvailable = localRegistrationAvailable,
            AllowedEmailDomainHint = localRegistrationAvailable ? "@hcltech.com" : null
        };
    }
}
