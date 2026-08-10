/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using HCL.CS.Domain.Constants;
using HCL.CS.DomainServices.Infra;

namespace HCL.CS.Infrastructure.Services.Implementation.Providers.Sms;

public class TwilioSmsProvider : ISmsProvider
{
    public string ProviderName => NotificationProviderConstants.Twilio;

    public async Task<ProviderSendResult> SendAsync(SmsMessage message, Dictionary<string, string> config)
    {
        try
        {
            var accountSid = config["AccountSid"];
            var authToken = config["AuthToken"];
            var fromNumber = !string.IsNullOrWhiteSpace(message.From) ? message.From : config["FromNumber"];

            TwilioClient.Init(accountSid, authToken);

            var createOptions = new CreateMessageOptions(new PhoneNumber(message.To))
            {
                From = new PhoneNumber(fromNumber),
                Body = message.Body
            };

            if (config.TryGetValue("StatusCallbackUrl", out var statusCallbackUrl)
                && !string.IsNullOrWhiteSpace(statusCallbackUrl))
            {
                createOptions.StatusCallback = new Uri(statusCallbackUrl);
            }
            else if (!string.IsNullOrWhiteSpace(message.CallbackUrl))
            {
                createOptions.StatusCallback = new Uri(message.CallbackUrl);
            }

            var result = await MessageResource.CreateAsync(createOptions);

            return new ProviderSendResult
            {
                Success = true,
                MessageId = result.Sid,
                ErrorMessage = string.Empty
            };
        }
        catch (Exception ex)
        {
            return new ProviderSendResult
            {
                Success = false,
                MessageId = string.Empty,
                ErrorMessage = $"Twilio SMS sending failed: {ex.Message}"
            };
        }
    }
}
