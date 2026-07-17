using Zentra.Domain.Models.Endpoint;

namespace Zentra.Service.Interfaces.Interfaces.Endpoint.Validators;

public interface IResourceScopeValidator
{
    Task<bool> ValidateRequestedScopeWithClientAsync(IList<string> clientScopes, IList<string> requestedScopes);

    Task<AllowedScopesParserModel> ValidateRequestedScopesAsync(ResourceScopeModel resourceScopeModel);
}
