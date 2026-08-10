/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using MessageBird;
using MessageBird.Objects;
using HCL.CS.Domain.Constants;
using HCL.CS.DomainServices.Infra;

namespace HCL.CS.Infrastructure.Services.Implementation.Providers.Sms;

public class MessageBirdSmsProvider : ISmsProvider
{
    public string ProviderName => NotificationProviderConstants.MessageBird;

    public async Task<ProviderSendResult> SendAsync(SmsMessage message, Dictionary<string, string> config)
    {
        try
        {
            var accessKey = config["AccessKey"];
            var originator = !string.IsNullOrWhiteSpace(message.From) ? message.From : config["Originator"];

            var client = Client.CreateDefault(accessKey);

            var result = await Task.Run(() =>
                client.SendMessage(originator, message.Body, new[] { long.Parse(message.To) })
            );

            return new ProviderSendResult
            {
                Success = true,
                MessageId = result.Id?.ToString() ?? string.Empty,
                ErrorMessage = string.Empty
            };
        }
        catch (Exception ex)
        {
            return new ProviderSendResult
            {
                Success = false,
                MessageId = string.Empty,
                ErrorMessage = $"MessageBird SMS sending failed: {ex.Message}"
            };
        }
    }
}
