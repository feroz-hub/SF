using Zentra.Domain.Configurations.Api;
using Zentra.Domain.Constants;

namespace Zentra.Domain.Models.Api;

public class NotificationSearchRequestModel
{
    public int? Type { get; set; }

    public int? Status { get; set; }

    public string FromDate { get; set; }

    public string ToDate { get; set; }

    public string SearchValue { get; set; }

    public PagingModel Page { get; set; }
}

public class NotificationLogModel
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string MessageId { get; set; }

    public int Type { get; set; }

    public string Activity { get; set; }

    public int Status { get; set; }

    public string Sender { get; set; }

    public string Recipient { get; set; }

    public DateTime CreatedOn { get; set; }
}

public class NotificationLogResponseModel
{
    public List<NotificationLogModel> Notifications { get; set; }

    public PagingModel PageInfo { get; set; }
}

public class NotificationTemplateResponseModel
{
    public List<EmailTemplate> EmailTemplates { get; set; }

    public List<SMSTemplate> SmsTemplates { get; set; }
}

public class ProviderConfigModel
{
    public Guid? Id { get; set; }

    public string ProviderName { get; set; }

    public int ChannelType { get; set; }

    public bool IsActive { get; set; }

    public Dictionary<string, string> Settings { get; set; }

    public DateTime? LastTestedOn { get; set; }

    public bool? LastTestSuccess { get; set; }
}

public class SaveProviderConfigRequest
{
    public Guid? Id { get; set; }

    public string ProviderName { get; set; }

    public int ChannelType { get; set; }

    public bool IsActive { get; set; }

    public Dictionary<string, string> Settings { get; set; }
}

public class SetActiveProviderRequest
{
    public Guid Id { get; set; }
}

public class DeleteProviderConfigRequest
{
    public Guid Id { get; set; }
}

public class ProviderFieldDefinitionsResponse
{
    public Dictionary<string, ProviderFieldDefinition[]> EmailProviders { get; set; }

    public Dictionary<string, ProviderFieldDefinition[]> SmsProviders { get; set; }
}

public class SendTestNotificationRequest
{
    public int Type { get; set; }

    public string Recipient { get; set; }

    public Guid? ProviderConfigId { get; set; }

    public Guid? UserId { get; set; }
}
