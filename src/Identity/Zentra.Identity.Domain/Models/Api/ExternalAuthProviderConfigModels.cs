using Zentra.Domain.Constants;

namespace Zentra.Domain.Models.Api;

public class ExternalAuthProviderConfigModel
{
    public Guid? Id { get; set; }

    public string ProviderName { get; set; }

    public int ProviderType { get; set; }

    public bool IsEnabled { get; set; }

    public Dictionary<string, string> Settings { get; set; }

    public bool AutoProvisionEnabled { get; set; }

    public string AllowedDomains { get; set; }

    public DateTime? LastTestedOn { get; set; }

    public bool? LastTestSuccess { get; set; }
}

public class SaveExternalAuthProviderRequest
{
    public Guid? Id { get; set; }

    public string ProviderName { get; set; }

    public int ProviderType { get; set; }

    public bool IsEnabled { get; set; }

    public Dictionary<string, string> Settings { get; set; }

    public bool AutoProvisionEnabled { get; set; }

    public string AllowedDomains { get; set; }
}

public class DeleteExternalAuthProviderRequest
{
    public Guid Id { get; set; }
}

public class TestExternalAuthProviderRequest
{
    public Guid Id { get; set; }
}

public class ExternalAuthFieldDefinitionsResponse
{
    public Dictionary<string, ProviderFieldDefinition[]> Providers { get; set; }

    public Dictionary<string, Dictionary<string, string>> Defaults { get; set; }
}
