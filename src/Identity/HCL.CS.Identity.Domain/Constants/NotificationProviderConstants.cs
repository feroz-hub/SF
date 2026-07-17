namespace HCL.CS.Domain.Constants;

public static class NotificationProviderConstants
{
    // Email Providers
    public const string Smtp = "SMTP";
    public const string SendGrid = "SendGrid";
    public const string Brevo = "Brevo";
    public const string Resend = "Resend";
    public const string AmazonSes = "AmazonSES";
    public const string Mailgun = "Mailgun";
    public const string Postmark = "Postmark";

    // SMS Providers
    public const string Twilio = "Twilio";
    public const string BrevoSms = "BrevoSMS";
    public const string Vonage = "Vonage";
    public const string AmazonSns = "AmazonSNS";
    public const string MessageBird = "MessageBird";
    public const string Plivo = "Plivo";

    public static readonly Dictionary<string, ProviderFieldDefinition[]> EmailProviderFields = new()
    {
        [Smtp] = new[]
        {
            new ProviderFieldDefinition("SmtpServer", "SMTP Server", "text", true),
            new ProviderFieldDefinition("Port", "Port", "number", true),
            new ProviderFieldDefinition("UserName", "Username", "text", true),
            new ProviderFieldDefinition("Password", "Password", "password", true),
            new ProviderFieldDefinition("UseSsl", "Use SSL", "boolean", false),
            new ProviderFieldDefinition("FromAddress", "Default From Address", "email", true),
            new ProviderFieldDefinition("FromName", "Default From Name", "text", false)
        },
        [SendGrid] = new[]
        {
            new ProviderFieldDefinition("ApiKey", "API Key", "password", true),
            new ProviderFieldDefinition("FromAddress", "From Address", "email", true),
            new ProviderFieldDefinition("FromName", "From Name", "text", false)
        },
        [Brevo] = new[]
        {
            new ProviderFieldDefinition("ApiKey", "API Key", "password", true),
            new ProviderFieldDefinition("FromAddress", "From Address", "email", true),
            new ProviderFieldDefinition("FromName", "From Name", "text", false)
        },
        [Resend] = new[]
        {
            new ProviderFieldDefinition("ApiKey", "API Key", "password", true),
            new ProviderFieldDefinition("FromAddress", "From Address", "email", true),
            new ProviderFieldDefinition("FromName", "From Name", "text", false)
        },
        [AmazonSes] = new[]
        {
            new ProviderFieldDefinition("AccessKeyId", "Access Key ID", "text", true),
            new ProviderFieldDefinition("SecretAccessKey", "Secret Access Key", "password", true),
            new ProviderFieldDefinition("Region", "AWS Region", "text", true),
            new ProviderFieldDefinition("FromAddress", "From Address", "email", true),
            new ProviderFieldDefinition("FromName", "From Name", "text", false)
        },
        [Mailgun] = new[]
        {
            new ProviderFieldDefinition("ApiKey", "API Key", "password", true),
            new ProviderFieldDefinition("Domain", "Mailgun Domain", "text", true),
            new ProviderFieldDefinition("FromAddress", "From Address", "email", true),
            new ProviderFieldDefinition("FromName", "From Name", "text", false),
            new ProviderFieldDefinition("Region", "Region (US/EU)", "text", false)
        },
        [Postmark] = new[]
        {
            new ProviderFieldDefinition("ServerToken", "Server Token", "password", true),
            new ProviderFieldDefinition("FromAddress", "From Address", "email", true),
            new ProviderFieldDefinition("FromName", "From Name", "text", false)
        }
    };

    public static readonly Dictionary<string, ProviderFieldDefinition[]> SmsProviderFields = new()
    {
        [Twilio] = new[]
        {
            new ProviderFieldDefinition("AccountSid", "Account SID", "text", true),
            new ProviderFieldDefinition("AuthToken", "Auth Token", "password", true),
            new ProviderFieldDefinition("FromNumber", "From Phone Number", "text", true),
            new ProviderFieldDefinition("StatusCallbackUrl", "Status Callback URL", "text", false)
        },
        [BrevoSms] = new[]
        {
            new ProviderFieldDefinition("ApiKey", "API Key", "password", true),
            new ProviderFieldDefinition("SenderName", "Sender Name", "text", true)
        },
        [Vonage] = new[]
        {
            new ProviderFieldDefinition("ApiKey", "API Key", "text", true),
            new ProviderFieldDefinition("ApiSecret", "API Secret", "password", true),
            new ProviderFieldDefinition("FromNumber", "From Number/Name", "text", true)
        },
        [AmazonSns] = new[]
        {
            new ProviderFieldDefinition("AccessKeyId", "Access Key ID", "text", true),
            new ProviderFieldDefinition("SecretAccessKey", "Secret Access Key", "password", true),
            new ProviderFieldDefinition("Region", "AWS Region", "text", true),
            new ProviderFieldDefinition("SenderId", "Sender ID", "text", false)
        },
        [MessageBird] = new[]
        {
            new ProviderFieldDefinition("AccessKey", "Access Key", "password", true),
            new ProviderFieldDefinition("Originator", "Originator", "text", true)
        },
        [Plivo] = new[]
        {
            new ProviderFieldDefinition("AuthId", "Auth ID", "text", true),
            new ProviderFieldDefinition("AuthToken", "Auth Token", "password", true),
            new ProviderFieldDefinition("FromNumber", "From Number", "text", true)
        }
    };
}

public class ProviderFieldDefinition
{
    public string Key { get; set; }

    public string Label { get; set; }

    public string InputType { get; set; }

    public bool Required { get; set; }

    public ProviderFieldDefinition(string key, string label, string inputType, bool required)
    {
        Key = key;
        Label = label;
        InputType = inputType;
        Required = required;
    }
}
