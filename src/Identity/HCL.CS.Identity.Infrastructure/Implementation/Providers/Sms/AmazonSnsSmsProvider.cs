/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using HCL.CS.Domain.Constants;
using HCL.CS.DomainServices.Infra;

namespace HCL.CS.Infrastructure.Services.Implementation.Providers.Sms;

public class AmazonSnsSmsProvider : ISmsProvider
{
    public string ProviderName => NotificationProviderConstants.AmazonSns;

    public async Task<ProviderSendResult> SendAsync(SmsMessage message, Dictionary<string, string> config)
    {
        try
        {
            var accessKeyId = config["AccessKeyId"];
            var secretAccessKey = config["SecretAccessKey"];
            var region = config["Region"];

            using var client = new AmazonSimpleNotificationServiceClient(
                accessKeyId,
                secretAccessKey,
                RegionEndpoint.GetBySystemName(region)
            );

            var publishRequest = new PublishRequest
            {
                PhoneNumber = message.To,
                Message = message.Body
            };

            if (config.TryGetValue("SenderId", out var senderId) && !string.IsNullOrWhiteSpace(senderId))
            {
                publishRequest.MessageAttributes["AWS.SNS.SMS.SenderID"] = new MessageAttributeValue
                {
                    DataType = "String",
                    StringValue = senderId
                };
            }

            publishRequest.MessageAttributes["AWS.SNS.SMS.SMSType"] = new MessageAttributeValue
            {
                DataType = "String",
                StringValue = "Transactional"
            };

            var response = await client.PublishAsync(publishRequest);

            return new ProviderSendResult
            {
                Success = true,
                MessageId = response.MessageId,
                ErrorMessage = string.Empty
            };
        }
        catch (Exception ex)
        {
            return new ProviderSendResult
            {
                Success = false,
                MessageId = string.Empty,
                ErrorMessage = $"Amazon SNS SMS sending failed: {ex.Message}"
            };
        }
    }
}
