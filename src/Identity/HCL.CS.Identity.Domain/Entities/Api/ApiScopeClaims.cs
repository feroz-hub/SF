namespace HCL.CS.Domain.Entities.Api;

public class ApiScopeClaims : BaseEntity
{
    public virtual Guid ApiScopeId { get; set; }

    public virtual string Type { get; set; }

    public virtual ApiScopes ApiScope { get; set; }
}
