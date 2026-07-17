namespace Zentra.Domain.Entities.Api;

public class NotificationProviderConfig : BaseEntity
{
    public string ProviderName { get; set; }

    public int ChannelType { get; set; }

    public bool IsActive { get; set; }

    public string ConfigJson { get; set; }

    public DateTime? LastTestedOn { get; set; }

    public bool? LastTestSuccess { get; set; }
}
