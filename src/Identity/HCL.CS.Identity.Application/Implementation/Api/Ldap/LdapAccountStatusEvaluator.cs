/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Globalization;
using HCL.CS.Domain.Models.Api;
using HCL.CS.Service.Interfaces.Interfaces.Api;

namespace HCL.CS.Service.Implementation.Api.Ldap;

public sealed class LdapAccountStatusEvaluator : ILdapAccountStatusEvaluator
{
    private const long AccountDisabledFlag = 0x0002;
    private const long AccountLockedFlag = 0x0010;
    private const long PasswordExpiredFlag = 0x800000;

    public LdapAccountState Evaluate(object? rawStatus)
    {
        if (rawStatus is null) return LdapAccountState.Unknown;

        if (TryConvertToInt64(rawStatus, out var userAccountControl))
        {
            if ((userAccountControl & AccountDisabledFlag) != 0) return LdapAccountState.Disabled;
            if ((userAccountControl & AccountLockedFlag) != 0) return LdapAccountState.Locked;
            if ((userAccountControl & PasswordExpiredFlag) != 0) return LdapAccountState.Expired;
            return LdapAccountState.Active;
        }

        var value = Convert.ToString(rawStatus, CultureInfo.InvariantCulture)?.Trim();
        if (string.IsNullOrWhiteSpace(value)) return LdapAccountState.Unknown;

        if (value.Equals("active", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("enabled", StringComparison.OrdinalIgnoreCase))
            return LdapAccountState.Active;
        if (value.Equals("disabled", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("inactive", StringComparison.OrdinalIgnoreCase))
            return LdapAccountState.Disabled;
        if (value.Equals("locked", StringComparison.OrdinalIgnoreCase))
            return LdapAccountState.Locked;
        if (value.Equals("expired", StringComparison.OrdinalIgnoreCase))
            return LdapAccountState.Expired;

        return LdapAccountState.Unknown;
    }

    private static bool TryConvertToInt64(object value, out long result)
    {
        switch (value)
        {
            case byte number:
                result = number;
                return true;
            case short number:
                result = number;
                return true;
            case int number:
                result = number;
                return true;
            case long number:
                result = number;
                return true;
            default:
                return long.TryParse(
                    Convert.ToString(value, CultureInfo.InvariantCulture),
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out result);
        }
    }
}
