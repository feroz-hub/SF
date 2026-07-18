/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using SendGrid;
using SendGrid.Helpers.Mail;
using HCL.CS.Domain.Constants;
using HCL.CS.DomainServices.Infra;

namespace HCL.CS.Infrastructure.Services.Implementation.Providers.Email;

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
