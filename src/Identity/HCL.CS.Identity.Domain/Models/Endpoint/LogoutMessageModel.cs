namespace HCL.CS.Domain.Models.Endpoint;

public class LogoutMessageModel
{
    public string ClientId { get; set; }

    public string PostLogoutRedirectUri { get; set; }

    public string SubjectId { get; set; }

    public string SessionId { get; set; }

    public IEnumerable<string> ClientIdCollection { get; set; }

    public Dictionary<string, string> Parameters { get; set; } = new();

    public bool HasClient => !string.IsNullOrWhiteSpace(ClientId) || ClientIdCollection?.Any() == true;
}
