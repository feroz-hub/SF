using Zentra.Domain.Configurations.Api;
using Zentra.Domain.Configurations.Endpoint;

namespace Zentra.Domain;

public class ZentraConfig
{
    public SystemSettings SystemSettings { get; set; } = new();

    public NotificationTemplateSettings NotificationTemplateSettings { get; set; } = new();

    public TokenSettings TokenSettings { get; set; } = new();
}

public class SystemSettings
{
    public DBConfig DBConfig { get; set; } = new();

    public LoginConfig LoginConfig { get; set; } = new();

    public UserConfig UserConfig { get; set; } = new();

    public PasswordConfig PasswordConfig { get; set; } = new();

    public EmailConfig EmailConfig { get; set; } = new();

    public SMSConfig SMSConfig { get; set; } = new();

    public LdapConfig LdapConfig { get; set; } = new();

    public CryptoConfig CryptoConfig { get; set; } = new();

    public LogConfig LogConfig { get; set; } = new();
}

public class NotificationTemplateSettings
{
    public List<EmailTemplate> EmailTemplateCollection { get; set; } = new();

    public List<SMSTemplate> SMSTemplateCollection { get; set; } = new();
}

public class TokenSettings
{
    public TokenConfig TokenConfig { get; set; } = new();

    public AuthenticationConfig AuthenticationConfig { get; set; } = new();

    public InputLengthRestrictionsConfig InputLengthRestrictionsConfig { get; set; } = new();

    public UserInteractionConfig UserInteractionConfig { get; set; } = new();

    public EndpointsConfig EndpointsConfig { get; set; } = new();

    public TokenExpiration TokenExpiration { get; set; } = new();
}
