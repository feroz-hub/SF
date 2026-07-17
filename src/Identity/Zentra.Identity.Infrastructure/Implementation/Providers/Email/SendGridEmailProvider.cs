using SendGrid;
using SendGrid.Helpers.Mail;
using Zentra.Domain.Constants;
using Zentra.DomainServices.Infra;

namespace Zentra.Infrastructure.Services.Implementation.Providers.Email;

public class SendGridEmailProvider : IEmailProvider
{
    public string ProviderName => NotificationProviderConstants.SendGrid;

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

            var client = new SendGridClient(config["ApiKey"]);

            var from = new EmailAddress(fromAddress, fromName);
            var to = new EmailAddress(message.To);
            var msg = MailHelper.CreateSingleEmail(from, to, message.Subject, null, message.HtmlBody);

            if (!string.IsNullOrWhiteSpace(message.CC))
            {
                msg.AddCc(new EmailAddress(message.CC));
            }

            var response = await client.SendEmailAsync(msg);

            if (response.IsSuccessStatusCode)
            {
                var messageId = response.Headers.TryGetValues("X-Message-Id", out var values)
                    ? values.FirstOrDefault() ?? string.Empty
                    : string.Empty;

                return new ProviderSendResult
                {
                    Success = true,
                    MessageId = messageId,
                    ErrorMessage = string.Empty
                };
            }

            var errorBody = await response.Body.ReadAsStringAsync();
            return new ProviderSendResult
            {
                Success = false,
                MessageId = string.Empty,
                ErrorMessage = $"SendGrid returned {response.StatusCode}: {errorBody}"
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
