namespace Zentra.Domain.Models.Api;

public class NotificationInfoModel
{
    public Guid UserId { get; set; }

    public string ToAddress { get; set; }

    public string Activity { get; set; }

    public string TemplateName { get; set; }

    public Dictionary<string, string> Parameters { get; set; }
}
