using System.Security.Claims;
using HCL.CS.Domain.Models.Endpoint.Validation;

namespace HCL.CS.Domain.Models.Endpoint.Request;

public class ValidatedAuthorizeRequestModel : ValidatedBaseModel
{
    public ValidatedAuthorizeRequestModel()
    {
        RequestedScopes = new List<string>();
        AuthenticationContextReferenceClasses = new List<string>();
    }

    public string ResponseType { get; set; }

    public string ResponseMode { get; set; }

    public string GrantType { get; set; }

    public List<string> RequestedScopes { get; set; }

    public string State { get; set; }

    public bool IsOpenIdRequest { get; set; } = false;

    public bool IsApiResourceRequest { get; set; }

    public string Nonce { get; set; }

    public List<string> AuthenticationContextReferenceClasses { get; set; }

    public IEnumerable<string> PromptModes { get; set; } = Enumerable.Empty<string>();

    public int? MaxAge { get; set; }

    public string CodeChallenge { get; set; }

    public string CodeChallengeMethod { get; set; }

    public ClaimsPrincipal User { get; set; }
}
