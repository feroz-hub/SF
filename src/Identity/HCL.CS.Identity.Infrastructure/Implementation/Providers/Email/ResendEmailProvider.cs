/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using HCL.CS.Domain.Constants;
using HCL.CS.DomainServices.Infra;

namespace HCL.CS.Infrastructure.Services.Implementation.Providers.Email;

public class ResendEmailProvider : IEmailProvider
{
    private static readonly HttpClient HttpClient = new();

    public string ProviderName => NotificationProviderConstants.Resend;

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

            var payload = new Dictionary<string, object>
            {
                ["from"] = from,
                ["to"] = new[] { message.To },
                ["subject"] = message.Subject,
                ["html"] = message.HtmlBody
            };

            if (!string.IsNullOrWhiteSpace(message.CC))
            {
                payload["cc"] = new[] { message.CC };
            }

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails")
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", config["ApiKey"]);

            var response = await HttpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var responseDoc = JsonDocument.Parse(responseBody);
                var messageId = responseDoc.RootElement.TryGetProperty("id", out var idElement)
                    ? idElement.GetString() ?? string.Empty
                    : string.Empty;

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
                ErrorMessage = $"Resend returned {response.StatusCode}: {responseBody}"
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
