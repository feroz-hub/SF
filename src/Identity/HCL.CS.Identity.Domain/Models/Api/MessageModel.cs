namespace HCL.CS.Domain.Models.Api;

public class EmailMessageModel
{
    public Guid UserId { get; set; }

    public string FromAddress { get; set; }

    public string FromName { get; set; }

    public string ToAddress { get; set; }

    public string ToName { get; set; }

    public string Subject { get; set; }

    public string Content { get; set; }

    public string Activity { get; set; }

    public string TemplateName { get; set; }

    public string CC { get; set; }

    public Dictionary<string, string> Parameters { get; set; }
}

public class SMSMessage
{
    public Guid UserId { get; set; }

    public string To { get; set; }

    public string Content { get; set; }

    public string Activity { get; set; }

    public string TemplateName { get; set; }

    public Dictionary<string, string> Parameters { get; set; }
}
