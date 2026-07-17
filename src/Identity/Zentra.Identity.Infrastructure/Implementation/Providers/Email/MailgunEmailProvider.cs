using RestSharp;
using RestSharp.Authenticators;
using Zentra.Domain.Constants;
using Zentra.DomainServices.Infra;

namespace Zentra.Infrastructure.Services.Implementation.Providers.Email;

public class MailgunEmailProvider : IEmailProvider
{
    public string ProviderName => NotificationProviderConstants.Mailgun;

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

            var domain = config["Domain"];
            var region = config.GetValueOrDefault("Region", "US");
            var baseUrl = string.Equals(region, "EU", StringComparison.OrdinalIgnoreCase)
                ? "https://api.eu.mailgun.net/v3"
                : "https://api.mailgun.net/v3";

            var options = new RestClientOptions($"{baseUrl}/{domain}")
            {
                Authenticator = new HttpBasicAuthenticator("api", config["ApiKey"])
            };

            using var client = new RestClient(options);

            var request = new RestRequest("messages", Method.Post);
            request.AddParameter("from", from);
            request.AddParameter("to", message.To);
            request.AddParameter("subject", message.Subject);
            request.AddParameter("html", message.HtmlBody);

            if (!string.IsNullOrWhiteSpace(message.CC))
            {
                request.AddParameter("cc", message.CC);
            }

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                var messageId = string.Empty;
                if (!string.IsNullOrWhiteSpace(response.Content))
                {
                    var doc = System.Text.Json.JsonDocument.Parse(response.Content);
                    if (doc.RootElement.TryGetProperty("id", out var idElement))
                    {
                        messageId = idElement.GetString() ?? string.Empty;
                    }
                }

                return new ProviderSendResult
                {
                    Success = true,
                    MessageId = messageId,
                    ErrorMessage = string.Empty
                };
            }

            return new ProviderSendResult
            {
                Success = false,
                MessageId = string.Empty,
                ErrorMessage = $"Mailgun returned {response.StatusCode}: {response.Content}"
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
