/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Text;

namespace HCL.CS.Service.Implementation.Api.Ldap;

public static class LdapFilterEncoder
{
    public static string Escape(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var escaped = new StringBuilder(value.Length);
        foreach (var character in value)
        {
            switch (character)
            {
                case '\0':
                    escaped.Append(@"\00");
                    break;
                case '*':
                    escaped.Append(@"\2a");
                    break;
                case '(':
                    escaped.Append(@"\28");
                    break;
                case ')':
                    escaped.Append(@"\29");
                    break;
                case '\\':
                    escaped.Append(@"\5c");
                    break;
                default:
                    escaped.Append(character);
                    break;
            }
        }

        return escaped.ToString();
    }

    public static string EscapeBytes(ReadOnlySpan<byte> value)
    {
        var escaped = new StringBuilder(value.Length * 3);
        foreach (var item in value)
            escaped.Append('\\').Append(item.ToString("x2"));

        return escaped.ToString();
    }
}
