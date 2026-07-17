namespace HCL.CS.Domain.Models.Api;

public class ApiScopesModel : BaseModel
{
    public virtual Guid ApiResourceId { get; set; }

    public virtual string Name { get; set; }

    public virtual string DisplayName { get; set; }

    public virtual string Description { get; set; }

    public virtual bool Required { get; set; } = false;

    public virtual bool Emphasize { get; set; } = false;

    public virtual List<ApiScopeClaimsModel> ApiScopeClaims { get; set; }
}
