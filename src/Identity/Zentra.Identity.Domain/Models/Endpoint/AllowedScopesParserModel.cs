namespace Zentra.Domain.Models.Endpoint;

public class AllowedScopesParserModel
{
    public List<string> ParsedIdentityResources { get; set; }

    public List<string> ParsedApiResources { get; set; }

    public List<string> ParsedApiScopes { get; set; }

    public List<string> ParsedTransactionScopes { get; set; }

    public bool AllowOfflineAccess { get; set; }

    public bool CreateIdentityToken { get; set; }

    public List<string> InvalidScopes { get; set; }

    public TokenDetailsModel TokenDetails { get; set; }

    public List<string> ParsedAllowedScopes
    {
        get
        {
            var parsedAllowedScopes = new List<string>();
            if (ParsedIdentityResources != null) parsedAllowedScopes.AddRange(ParsedIdentityResources);

            if (ParsedApiScopes != null) parsedAllowedScopes.AddRange(ParsedApiScopes);

            // TODO Refactor this property into a method.
            return parsedAllowedScopes.Distinct().ToList();
        }
    }
}
