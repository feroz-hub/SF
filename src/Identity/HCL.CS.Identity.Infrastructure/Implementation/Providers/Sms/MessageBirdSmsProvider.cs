using MessageBird;
using MessageBird.Objects;
using HCL.CS.Domain.Constants;
using HCL.CS.DomainServices.Infra;

namespace HCL.CS.Infrastructure.Services.Implementation.Providers.Sms;

public class MessageBirdSmsProvider : ISmsProvider
{
    public string ProviderName => NotificationProviderConstants.MessageBird;

    public async Task<ProviderSendResult> SendAsync(SmsMessage message, Dictionary<string, string> config)
    {
        try
        {
            var accessKey = config["AccessKey"];
            var originator = !string.IsNullOrWhiteSpace(message.From) ? message.From : config["Originator"];

            var client = Client.CreateDefault(accessKey);

            var result = await Task.Run(() =>
                client.SendMessage(originator, message.Body, new[] { long.Parse(message.To) })
            );

            return new ProviderSendResult
            {
                Success = true,
                MessageId = result.Id?.ToString() ?? string.Empty,
                ErrorMessage = string.Empty
            };
        }
        catch (Exception ex)
        {
            return new ProviderSendResult
            {
                Success = false,
                MessageId = string.Empty,
                ErrorMessage = $"MessageBird SMS sending failed: {ex.Message}"
            };
        }
    }
}
