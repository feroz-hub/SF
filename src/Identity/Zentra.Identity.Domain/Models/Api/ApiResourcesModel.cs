namespace Zentra.Domain.Models.Api;

public class ApiResourcesModel : BaseModel
{
    public virtual string Name { get; set; }

    public virtual string DisplayName { get; set; }

    public virtual string Description { get; set; }

    public virtual bool Enabled { get; set; } = true;

    public virtual List<ApiResourceClaimsModel> ApiResourceClaims { get; set; }

    public virtual List<ApiScopesModel> ApiScopes { get; set; }
}
