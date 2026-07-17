namespace HCL.CS.Domain.Constants;

public static class ExternalAuthProviderConstants
{
    public const string Google = "Google";

    public static readonly Dictionary<string, ProviderFieldDefinition[]> ProviderFields = new()
    {
        [Google] = new[]
        {
            new ProviderFieldDefinition("ClientId", "Client ID", "text", true),
            new ProviderFieldDefinition("ClientSecret", "Client Secret", "password", true),
            new ProviderFieldDefinition("Authority", "Authority", "text", false),
            new ProviderFieldDefinition("MetadataAddress", "Metadata Address", "text", false),
            new ProviderFieldDefinition("CallbackPath", "Callback Path", "text", false),
            new ProviderFieldDefinition("AllowedRedirectHosts", "Allowed Redirect Hosts", "textarea", false)
        }
    };

    public static readonly Dictionary<string, Dictionary<string, string>> ProviderDefaults = new()
    {
        [Google] = new Dictionary<string, string>
        {
            { "Authority", "https://accounts.google.com" },
            { "MetadataAddress", "https://accounts.google.com/.well-known/openid-configuration" },
            { "CallbackPath", "/auth/external/google/signin-callback" }
        }
    };
}
