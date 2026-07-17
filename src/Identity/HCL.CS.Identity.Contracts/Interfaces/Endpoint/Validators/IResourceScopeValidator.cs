using HCL.CS.Domain.Models.Endpoint;

namespace HCL.CS.Service.Interfaces.Interfaces.Endpoint.Validators;

public interface IResourceScopeValidator
{
    Task<bool> ValidateRequestedScopeWithClientAsync(IList<string> clientScopes, IList<string> requestedScopes);

    Task<AllowedScopesParserModel> ValidateRequestedScopesAsync(ResourceScopeModel resourceScopeModel);
}
