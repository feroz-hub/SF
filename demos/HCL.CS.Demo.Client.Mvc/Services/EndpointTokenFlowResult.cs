namespace HCL.CS.DemoClientMvc.Services;

public sealed class EndpointTokenFlowResult
{
    public string? AccessToken { get; set; }

    public string? IdToken { get; set; }

    public string? RefreshToken { get; set; }

    public int ExpiresIn { get; set; }

    public string? TokenType { get; set; }

    public string? ErrorCode { get; set; }

    public string? ErrorDescription { get; set; }

    public IReadOnlyDictionary<string, string> Claims { get; set; } = new Dictionary<string, string>();

    public bool Succeeded => !string.IsNullOrWhiteSpace(AccessToken) && string.IsNullOrWhiteSpace(ErrorCode);
}
