namespace HCL.CS.Domain.Entities.Api;

public class ApiScopes : BaseEntity
{
    public virtual Guid ApiResourceId { get; set; }

    public virtual string Name { get; set; }

    public virtual string DisplayName { get; set; }

    public virtual string Description { get; set; }

    public virtual bool Required { get; set; }

    public virtual bool Emphasize { get; set; }

    public virtual ApiResources ApiResource { get; set; }

    public virtual List<ApiScopeClaims> ApiScopeClaims { get; set; }
}
