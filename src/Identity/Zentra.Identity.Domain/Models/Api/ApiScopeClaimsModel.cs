namespace Zentra.Domain.Models.Api;

public class ApiScopeClaimsModel : BaseModel
{
    public virtual Guid ApiScopeId { get; set; }

    public virtual string Type { get; set; }
}
