/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using IdentityModel;
using Microsoft.AspNetCore.DataProtection;

namespace HCL.CS.Service.Implementation.Endpoint.Extensions;

internal static class DataProtectionExtension
{
    private const string ProtectorPurpose = "CyberHclCs";
    private const string ApplicationName = "HclCsFramework";

    public static async Task<string> ProtectDataAsync<T>(this T message)
    {
        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        var dataProtectionProvider = DataProtectionProvider.Create(ApplicationName);
        var protector = dataProtectionProvider.CreateProtector(ProtectorPurpose);
        var json = JsonSerializer.Serialize(message, options);
        var bytes = Encoding.UTF8.GetBytes(json);
        bytes = protector.Protect(bytes);
        var value = Base64Url.Encode(bytes);
        return await Task.FromResult(value);
    }

    public static async Task<T> UnProtectDataAsync<T>(this string value)
    {
        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        var dataProtectionProvider = DataProtectionProvider.Create(ApplicationName);
        var protector = dataProtectionProvider.CreateProtector(ProtectorPurpose);
        var bytes = Base64Url.Decode(value);
        bytes = protector.Unprotect(bytes);
        var json = Encoding.UTF8.GetString(bytes);
        var result = JsonSerializer.Deserialize<T>(json, options);
        return await Task.FromResult(result);
    }
}
