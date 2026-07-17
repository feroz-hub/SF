using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;
using HCL.CS.Domain.Constants;
using HCL.CS.DomainServices.Infra;

namespace HCL.CS.Infrastructure.Services.Implementation.Providers.Email;

public class BrevoEmailProvider : IEmailProvider
{
    public string ProviderName => NotificationProviderConstants.Brevo;

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

            var configuration = new Configuration();
            configuration.ApiKey.Add("api-key", config["ApiKey"]);

            var apiInstance = new TransactionalEmailsApi(configuration);

            var sendSmtpEmail = new SendSmtpEmail
            {
                Sender = new SendSmtpEmailSender(fromName, fromAddress),
                To = new List<SendSmtpEmailTo> { new(message.To) },
                Subject = message.Subject,
                HtmlContent = message.HtmlBody
            };

            if (!string.IsNullOrWhiteSpace(message.CC))
            {
                sendSmtpEmail.Cc = new List<SendSmtpEmailCc> { new(message.CC) };
            }

            var result = await apiInstance.SendTransacEmailAsync(sendSmtpEmail);

            return new ProviderSendResult
            {
                Success = true,
                MessageId = result.MessageId ?? string.Empty,
                ErrorMessage = string.Empty
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
