using PostmarkDotNet;
using HCL.CS.Domain.Constants;
using HCL.CS.DomainServices.Infra;

namespace HCL.CS.Infrastructure.Services.Implementation.Providers.Email;

public class PostmarkEmailProvider : IEmailProvider
{
    public string ProviderName => NotificationProviderConstants.Postmark;

    public async Task<ProviderSendResult> SendAsync(EmailMessage message, Dictionary<string, string> config)
    {
        try
        {
            var fromAddress = !string.IsNullOrWhiteSpace(message.From)
                ? message.From
                : config.GetValueOrDefault("FromAddress", string.Empty);

            var fromName = !string.IsNullOrWhiteSpace(message.FromName)
                ? message.FromName
                : config.GetValueOrDefault("FromName", string.Empty);

            var from = !string.IsNullOrWhiteSpace(fromName)
                ? $"{fromName} <{fromAddress}>"
                : fromAddress;

            var client = new PostmarkClient(config["ServerToken"]);

            var postmarkMessage = new PostmarkMessage
            {
                From = from,
                To = message.To,
                Subject = message.Subject,
                HtmlBody = message.HtmlBody
            };

            if (!string.IsNullOrWhiteSpace(message.CC))
            {
                postmarkMessage.Cc = message.CC;
            }

            var response = await client.SendMessageAsync(postmarkMessage);

            if (response.Status == PostmarkStatus.Success)
            {
                return new ProviderSendResult
                {
                    Success = true,
                    MessageId = response.MessageID.ToString(),
                    ErrorMessage = string.Empty
                };
            }

            return new ProviderSendResult
            {
                Success = false,
                MessageId = string.Empty,
                ErrorMessage = $"Postmark error: {response.Message}"
            };
        }
        catch (Exception ex)
        {
            return new ProviderSendResult
            {
                Success = false,
                MessageId = string.Empty,
                ErrorMessage = ex.Message
            };
        }
    }
}
