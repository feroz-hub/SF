using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Zentra.Domain.Constants;
using Zentra.DomainServices.Infra;

namespace Zentra.Infrastructure.Services.Implementation.Providers.Sms;

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
