namespace Zentra.Domain.Entities.Api;

public class ApiResources : BaseEntity
{
    public virtual string Name { get; set; }

    public virtual string DisplayName { get; set; }

    public virtual string Description { get; set; }

    public virtual bool Enabled { get; set; }

    public virtual List<ApiResourceClaims> ApiResourceClaims { get; set; }

    public virtual List<ApiScopes> ApiScopes { get; set; }
}
