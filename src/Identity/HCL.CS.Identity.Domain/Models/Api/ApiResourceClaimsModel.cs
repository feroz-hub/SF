namespace HCL.CS.Domain.Models.Api;

public class ApiResourceClaimsModel : BaseModel
{
    public virtual Guid ApiResourceId { get; set; }

    public virtual string Type { get; set; }
}
