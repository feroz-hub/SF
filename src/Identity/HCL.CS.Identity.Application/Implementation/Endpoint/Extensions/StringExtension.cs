using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.Models.Endpoint.Validation;

namespace HCL.CS.Service.Implementation.Endpoint.Extensions;

internal static class StringExtension
{
    internal static string IncludedFrontSlash(this string url)
    {
        if (url != null && !url.StartsWith("/")) return "/" + url;

        return url;
    }

    internal static string IncludeEndSlash(this string url)
    {
        if (url != null && !url.EndsWith("/")) return url + "/";

        return url;
    }

    internal static string RemoveFrontSlash(this string url)
    {
        if (url != null && url.StartsWith("/")) url = url[1..];

        return url;
    }

    internal static string RemoveBackSlash(this string url)
    {
        if (url != null && url.EndsWith("/")) url = url[..^1];

        return url;
    }

    internal static string GetSpaceSeparatedValues(this Type type)
    {
        var authenticationMethods = type
            .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly) // constants, not readonly
            .Where(fi => fi.FieldType == typeof(string)) // of type string
            .ToDictionary(fi => fi.Name, fi => fi.GetValue(null) as string);
        return authenticationMethods.Values.AsEnumerable().ConvertSpaceSeparatedString();
    }

    internal static string[] GetArray(this Type type)
    {
        var authenticationMethods = type
            .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly) // constants, not readonly
            .Where(fi => fi.FieldType == typeof(string)) // of type string
            .ToDictionary(fi => fi.Name, fi => fi.GetValue(null) as string);
        return authenticationMethods.Values.ToArray();
    }

    internal static bool CompareStrings(this string string1, string string2)
    {
        if (string1 == null && string2 == null) return true;

        if (string1 == null || string2 == null) return false;

        var string1Bytes = Encoding.UTF8.GetBytes(string1);
        var string2Bytes = Encoding.UTF8.GetBytes(string2);
        if (string1Bytes.Length != string2Bytes.Length) return false;

        return CryptographicOperations.FixedTimeEquals(string1Bytes, string2Bytes);
    }

    internal static string ConvertSpaceSeparatedString(this IEnumerable<string> list)
    {
        if (list == null) return string.Empty;

        var sb = new StringBuilder(100);
        foreach (var element in list) sb.Append(element + " ");

        return sb.ToString().Trim();
    }

    internal static string ConvertSpaceSeparatedString(this IEnumerable<Claim> list)
    {
        if (list == null) return string.Empty;

        var sb = new StringBuilder(100);
        foreach (var element in list) sb.Append(element.Value + " ");

        return sb.ToString().Trim();
    }

    internal static IEnumerable<string> ParseScopesString(this string scopes)
    {
        if (string.IsNullOrWhiteSpace(scopes)) return Enumerable.Empty<string>();

        scopes = scopes.Trim();
        var parsedScopes = scopes.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
        if (parsedScopes.ContainsAny())
        {
            parsedScopes.Sort();
            return parsedScopes;
        }

        return Enumerable.Empty<string>();
    }

    internal static IEnumerable<string> SplitBySpace(this string input)
    {
        if (!string.IsNullOrWhiteSpace(input))
        {
            input = input.Trim();
            return input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        return Enumerable.Empty<string>();
    }

    internal static string GetValueFromDictionary(this Dictionary<string, string> requestCollection, string key)
    {
        if (!string.IsNullOrWhiteSpace(key) && requestCollection.ContainsKey(key) &&
            requestCollection.TryGetValue(key, out var value)) return value.Trim();

        return null;
    }

    internal static string GetValue(this ValidatedBaseModel request, string key)
    {
        if (!string.IsNullOrWhiteSpace(key) && request.RequestRawData.ContainsKey(key) &&
            request.RequestRawData.TryGetValue(key, out var value)) return value.Trim();

        return string.Empty;
    }

    internal static DateTime? GetAuthenticationTimeFromIdentity(this IPrincipal principal)
    {
        if (principal != null && principal.Identity != null)
        {
            var id = principal.Identity as ClaimsIdentity;
            var claim = id?.FindFirst(OpenIdConstants.ClaimTypes.AuthenticationTime);
            if (claim != null)
            {
                var value = long.Parse(claim.Value);
                return DateTimeOffset.FromUnixTimeSeconds(value).UtcDateTime;
            }
        }

        return null;
    }

    internal static bool CheckLocalUrl(this string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;

        if (url[0] == '/')
        {
            if (url.Length == 1) return true;

            if (url[1] != '/' && url[1] != '\\') return true;

            return false;
        }

        if (url[0] == '~' && url.Length > 1 && url[1] == '/')
        {
            if (url.Length == 2) return true;

            if (url[2] != '/' && url[2] != '\\') return true;

            return false;
        }

        return false;
    }

    internal static bool IsExpired(this DateTime createdTime, long expiryDuration)
    {
        if (createdTime.AddSeconds(expiryDuration) < DateTime.UtcNow) return true;

        return false;
    }

    internal static bool IsAuthenticated(this IPrincipal principal)
    {
        return principal != null && principal.Identity != null && principal.Identity.IsAuthenticated;
    }

    internal static bool ContainsAny<T>(this IEnumerable<T> data)
    {
        return data != null && data.Any();
    }

    internal static bool IsValidUrl(this string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;

        if (url.Contains('*', StringComparison.Ordinal)) return false;

        if (!Uri.TryCreate(url, UriKind.Absolute, out var validatedUri)) return false;

        var allowInsecureHttpDev = string.Equals(
            Environment.GetEnvironmentVariable("HCL_CS_ALLOW_INSECURE_HTTP_DEV"),
            "true",
            StringComparison.OrdinalIgnoreCase);

        var isCustomScheme = validatedUri.Scheme != Uri.UriSchemeHttps
                             && validatedUri.Scheme != Uri.UriSchemeHttp;

        return validatedUri.IsWellFormedOriginalString()
               && validatedUri.IsAbsoluteUri
               && (validatedUri.Scheme == Uri.UriSchemeHttps
                   || (allowInsecureHttpDev && validatedUri.Scheme == Uri.UriSchemeHttp)
                   || (allowInsecureHttpDev && isCustomScheme && !string.IsNullOrWhiteSpace(validatedUri.Host)))
               && !string.IsNullOrWhiteSpace(validatedUri.Host);
    }

    internal static bool IsNull<T>(this IEnumerable<T> data)
    {
        if (data != null && data.Any())
        {
            foreach (var item in data)
                if (item == null)
                    return true;

            return false;
        }

        return true;
    }

    internal static List<string> ExpandPermissions(this List<string> permissions)
    {
        var newPermissionsList = new List<string>();
        if (permissions.ContainsAny())
            foreach (var scope in permissions)
                if (scope.Contains(PermissionConstants.Manage))
                {
                    newPermissionsList.Add(scope.Replace(PermissionConstants.Manage, PermissionConstants.Read));
                    newPermissionsList.Add(scope.Replace(PermissionConstants.Manage, PermissionConstants.Write));
                    newPermissionsList.Add(scope.Replace(PermissionConstants.Manage, PermissionConstants.Delete));
                }
                else
                {
                    newPermissionsList.Add(scope);
                }

        return newPermissionsList.Distinct().ToList();
    }

    internal static List<string> ShrinkPermissions(this List<string> permissions)
    {
        var newPermissionsList = new List<string>();
        var scopeSeparator = ".";
        if (permissions.ContainsAny())
            foreach (var scope in permissions)
            {
                var splitScope = scope.Split(scopeSeparator);
                var scopePrefix = splitScope[0] + scopeSeparator + splitScope[1];
                var readScope = scopePrefix + PermissionConstants.Read;
                var writeScope = scopePrefix + PermissionConstants.Write;
                var deleteScope = scopePrefix + PermissionConstants.Delete;
                var manageScope = scopePrefix + PermissionConstants.Manage;
                if (permissions.Contains(readScope) && permissions.Contains(writeScope) &&
                    permissions.Contains(deleteScope))
                {
                    newPermissionsList.Add(manageScope);
                }
                else
                {
                    if (permissions.Contains(manageScope) && permissions.Contains(scope))
                        newPermissionsList.Add(manageScope);
                    else
                        newPermissionsList.Add(scope);
                }
            }

        return newPermissionsList.Distinct().ToList();
    }
}
