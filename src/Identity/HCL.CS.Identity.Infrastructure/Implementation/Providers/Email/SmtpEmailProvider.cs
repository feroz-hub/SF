/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using HCL.CS.Domain.Constants;
using HCL.CS.DomainServices.Infra;

namespace HCL.CS.Infrastructure.Services.Implementation.Providers.Email;

public class SmtpEmailProvider : IEmailProvider
{
    public string ProviderName => NotificationProviderConstants.Smtp;

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

            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(fromName, fromAddress));
            mimeMessage.To.Add(MailboxAddress.Parse(message.To));
            mimeMessage.Subject = message.Subject;

            if (!string.IsNullOrWhiteSpace(message.CC))
            {
                mimeMessage.Cc.Add(MailboxAddress.Parse(message.CC));
            }

            var bodyBuilder = new BodyBuilder { HtmlBody = message.HtmlBody };
            mimeMessage.Body = bodyBuilder.ToMessageBody();

            var server = config["SmtpServer"];
            var port = int.Parse(config["Port"]);
            var userName = config["UserName"];
            var password = config["Password"];
            var useSsl = config.TryGetValue("UseSsl", out var sslValue)
                && bool.TryParse(sslValue, out var ssl) && ssl;

            var secureSocketOptions = useSsl
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTls;

            using var client = new SmtpClient();
            await client.ConnectAsync(server, port, secureSocketOptions);
            await client.AuthenticateAsync(userName, password);
            var result = await client.SendAsync(mimeMessage);
            await client.DisconnectAsync(true);

            return new ProviderSendResult
            {
                Success = true,
                MessageId = mimeMessage.MessageId,
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
