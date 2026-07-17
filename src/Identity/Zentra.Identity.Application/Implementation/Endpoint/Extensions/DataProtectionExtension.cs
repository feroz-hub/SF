using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using IdentityModel;
using Microsoft.AspNetCore.DataProtection;

namespace Zentra.Service.Implementation.Endpoint.Extensions;

internal static class DataProtectionExtension
{
    private const string ProtectorPurpose = "CyberZentra";
    private const string ApplicationName = "ZentraFramework";

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
