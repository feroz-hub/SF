using Plivo;
using HCL.CS.Domain.Constants;
using HCL.CS.DomainServices.Infra;

namespace HCL.CS.Infrastructure.Services.Implementation.Providers.Sms;

public class PlivoSmsProvider : ISmsProvider
{
    public string ProviderName => NotificationProviderConstants.Plivo;

    public async Task<ProviderSendResult> SendAsync(SmsMessage message, Dictionary<string, string> config)
    {
        try
        {
            var authId = config["AuthId"];
            var authToken = config["AuthToken"];
            var fromNumber = !string.IsNullOrWhiteSpace(message.From) ? message.From : config["FromNumber"];

            var client = new PlivoApi(authId, authToken);

            var response = await Task.Run(() =>
                client.Message.Create(
                    src: fromNumber,
                    dst: new List<string> { message.To },
                    text: message.Body,
                    url: !string.IsNullOrWhiteSpace(message.CallbackUrl) ? message.CallbackUrl : null
                )
            );

            return new ProviderSendResult
            {
                Success = true,
                MessageId = response.MessageUuid?.FirstOrDefault() ?? string.Empty,
                ErrorMessage = string.Empty
            };
        }
        catch (Exception ex)
        {
            return new ProviderSendResult
            {
                Success = false,
                MessageId = string.Empty,
                ErrorMessage = $"Plivo SMS sending failed: {ex.Message}"
            };
        }
    }
}
