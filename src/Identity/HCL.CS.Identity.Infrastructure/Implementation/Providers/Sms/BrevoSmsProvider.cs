/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;
using HCL.CS.Domain.Constants;
using HCL.CS.DomainServices.Infra;

namespace HCL.CS.Infrastructure.Services.Implementation.Providers.Sms;

public class BrevoSmsProvider : ISmsProvider
{
    public string ProviderName => NotificationProviderConstants.BrevoSms;

    public async Task<ProviderSendResult> SendAsync(SmsMessage message, Dictionary<string, string> config)
    {
        try
        {
            var apiKey = config["ApiKey"];
            var senderName = !string.IsNullOrWhiteSpace(message.From) ? message.From : config["SenderName"];

            var configuration = new Configuration();
            configuration.AddApiKey("api-key", apiKey);

            var apiInstance = new TransactionalSMSApi(configuration);

            var sendTransacSms = new SendTransacSms(
                sender: senderName,
                recipient: message.To,
                content: message.Body
            );

            if (!string.IsNullOrWhiteSpace(message.CallbackUrl))
            {
                sendTransacSms.WebUrl = message.CallbackUrl;
            }

            var result = await apiInstance.SendTransacSmsAsync(sendTransacSms);

            return new ProviderSendResult
            {
                Success = true,
                MessageId = result.MessageId.ToString(),
                ErrorMessage = string.Empty
            };
        }
        catch (Exception ex)
        {
            return new ProviderSendResult
            {
                Success = false,
                MessageId = string.Empty,
                ErrorMessage = $"Brevo SMS sending failed: {ex.Message}"
            };
        }
    }
}
