namespace Zentra.DomainServices.Infra;

public interface IEmailProvider
{
    string ProviderName { get; }

    Task<ProviderSendResult> SendAsync(EmailMessage message, Dictionary<string, string> config);
}

public class EmailMessage
{
    public string From { get; set; }

    public string FromName { get; set; }

    public string To { get; set; }

    public string Subject { get; set; }

    public string HtmlBody { get; set; }

    public string CC { get; set; }
}

public class ProviderSendResult
{
    public bool Success { get; set; }

    public string MessageId { get; set; }

    public string ErrorMessage { get; set; }
}
