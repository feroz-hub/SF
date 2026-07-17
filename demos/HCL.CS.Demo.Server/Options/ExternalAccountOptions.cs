namespace HCL.CS.DemoServerApp.Options;

public sealed class ExternalAccountOptions
{
    public const string SectionName = "Authentication:ExternalAccount";

    public bool AutoProvisionEnabled { get; set; }

    public string[] AllowedDomains { get; set; } = Array.Empty<string>();

    public Dictionary<string, string[]> AllowedDomainsByTenant { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
