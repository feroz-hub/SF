namespace Zentra.Domain.Entities.Api;

public class ExternalAuthProviderConfig : BaseEntity
{
    public string ProviderName { get; set; }

    public int ProviderType { get; set; }

    public bool IsEnabled { get; set; }

    public string ConfigJson { get; set; }

    public bool AutoProvisionEnabled { get; set; }

    public string AllowedDomains { get; set; }

    public DateTime? LastTestedOn { get; set; }

    public bool? LastTestSuccess { get; set; }
}
