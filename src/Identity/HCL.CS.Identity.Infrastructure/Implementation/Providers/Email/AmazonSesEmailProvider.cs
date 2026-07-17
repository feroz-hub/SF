using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using HCL.CS.Domain.Constants;
using HCL.CS.DomainServices.Infra;

namespace HCL.CS.Infrastructure.Services.Implementation.Providers.Email;

public class AmazonSesEmailProvider : IEmailProvider
{
    public string ProviderName => NotificationProviderConstants.AmazonSes;

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

            var source = !string.IsNullOrWhiteSpace(fromName)
                ? $"{fromName} <{fromAddress}>"
                : fromAddress;

            var region = RegionEndpoint.GetBySystemName(config["Region"]);

            using var client = new AmazonSimpleEmailServiceClient(
                config["AccessKeyId"],
                config["SecretAccessKey"],
                region);

            var sendRequest = new SendEmailRequest
            {
                Source = source,
                Destination = new Destination
                {
                    ToAddresses = new List<string> { message.To }
                },
                Message = new Message
                {
                    Subject = new Content(message.Subject),
                    Body = new Body
                    {
                        Html = new Content(message.HtmlBody)
                    }
                }
            };

            if (!string.IsNullOrWhiteSpace(message.CC))
            {
                sendRequest.Destination.CcAddresses = new List<string> { message.CC };
            }

            var response = await client.SendEmailAsync(sendRequest);

            return new ProviderSendResult
            {
                Success = true,
                MessageId = response.MessageId ?? string.Empty,
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
