namespace HCL.CS.Domain.Models.Api;

public class ApiResourcesByScopesModel : BaseModel
{
    public virtual Guid ApiResourceId { get; set; }

    public virtual string ApiResourceName { get; set; }

    public virtual string ApiResourceClaimType { get; set; }

    public virtual string ApiScopeName { get; set; }

    public virtual string ApiScopeClaimType { get; set; }
}
