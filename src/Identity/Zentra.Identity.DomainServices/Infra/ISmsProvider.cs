namespace Zentra.DomainServices.Infra;

public interface ISmsProvider
{
    string ProviderName { get; }

    Task<ProviderSendResult> SendAsync(SmsMessage message, Dictionary<string, string> config);
}

public class SmsMessage
{
    public string From { get; set; }

    public string To { get; set; }

    public string Body { get; set; }

    public string CallbackUrl { get; set; }
}
