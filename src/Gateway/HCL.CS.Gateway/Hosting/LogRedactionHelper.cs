using System.Text.RegularExpressions;

namespace HCL.CS.ProxyService.Hosting;

internal static partial class LogRedactionHelper
{
    private const string RedactedValue = "[REDACTED]";
    private static readonly HashSet<string> SensitiveFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "password",
        "secret",
        "token",
        "authorization",
        "cookie",
        "apikey",
        "api_key",
        "email",
        "phone",
        "ssn"
    };

    internal static string RedactByFieldName(string fieldName, string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        return IsSensitiveField(fieldName) ? RedactedValue : value.Trim();
    }

    internal static string GetSafeTenantId(string? tenantId)
    {
        if (string.IsNullOrWhiteSpace(tenantId)) return string.Empty;
        return RedactByFieldName("tenantId", tenantId);
    }

    internal static string GetSafeUserId(string? userId)
    {
        if (string.IsNullOrWhiteSpace(userId)) return "anonymous";

        var trimmed = userId.Trim();
        if (trimmed.Length > 64) return RedactedValue;
        if (trimmed.Contains('@', StringComparison.Ordinal)) return RedactedValue;
        if (trimmed.Any(char.IsWhiteSpace)) return RedactedValue;
        if (PhoneLikeRegex().IsMatch(trimmed)) return RedactedValue;

        return trimmed;
    }

    private static bool IsSensitiveField(string fieldName)
    {
        if (string.IsNullOrWhiteSpace(fieldName)) return false;
        return SensitiveFields.Any(s => fieldName.Contains(s, StringComparison.OrdinalIgnoreCase));
    }

    [GeneratedRegex(@"^\+?\d{7,15}$", RegexOptions.Compiled)]
    private static partial Regex PhoneLikeRegex();
}
