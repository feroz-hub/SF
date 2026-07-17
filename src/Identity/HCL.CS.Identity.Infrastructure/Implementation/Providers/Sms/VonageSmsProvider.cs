using Vonage;
using Vonage.Messaging;
using Vonage.Request;
using HCL.CS.Domain.Constants;
using HCL.CS.DomainServices.Infra;

namespace HCL.CS.Infrastructure.Services.Implementation.Providers.Sms;

public class VonageSmsProvider : ISmsProvider
{
    public string ProviderName => NotificationProviderConstants.Vonage;

    public async Task<ProviderSendResult> SendAsync(SmsMessage message, Dictionary<string, string> config)
    {
        try
        {
            var apiKey = config["ApiKey"];
            var apiSecret = config["ApiSecret"];
            var fromNumber = !string.IsNullOrWhiteSpace(message.From) ? message.From : config["FromNumber"];

            var credentials = Credentials.FromApiKeyAndSecret(apiKey, apiSecret);
            var client = new VonageClient(credentials);

            var response = await client.SmsClient.SendAnSmsAsync(new SendSmsRequest
            {
                From = fromNumber,
                To = message.To,
                Text = message.Body
            });

            var firstMessage = response.Messages?.FirstOrDefault();

            if (firstMessage?.Status == "0")
            {
                return new ProviderSendResult
                {
                    Success = true,
                    MessageId = firstMessage.MessageId,
                    ErrorMessage = string.Empty
                };
            }

            return new ProviderSendResult
            {
                Success = false,
                MessageId = string.Empty,
                ErrorMessage = $"Vonage SMS failed: {firstMessage?.ErrorText ?? "Unknown error"}"
            };
        }
        catch (Exception ex)
        {
            return new ProviderSendResult
            {
                Success = false,
                MessageId = string.Empty,
                ErrorMessage = $"Vonage SMS sending failed: {ex.Message}"
            };
        }
    }
}
