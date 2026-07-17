using System.Security.Claims;

namespace HCL.CS.Domain.Models.Endpoint;

public class AuthorizationCodeModel
{
    public virtual Guid Id { get; set; } = default;

    public virtual string State { get; set; }

    public virtual string ClientId { get; set; }

    public virtual string RedirectUri { get; set; }

    public bool IsOpenId { get; set; }

    public DateTime CreationTime { get; set; }

    public int Lifetime { get; set; }

    public ClaimsPrincipal Subject { get; set; }

    public string Nonce { get; set; }

    public string SessionId { get; set; }

    public string CodeChallenge { get; set; }

    public string CodeChallengeMethod { get; set; }

    public List<string> RequestedScopes { get; set; }

    public AllowedScopesParserModel AllowedScopesParserModel { get; set; }

    public TokenDetailsModel TokenDetails { get; set; }
}
