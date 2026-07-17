using Microsoft.AspNetCore.Identity;

namespace HCL.CS.Domain.Entities.Api;

public class Roles : IdentityRole<Guid>
{
    public string Description { get; set; }

    public virtual bool IsDeleted { get; set; }

    public virtual DateTime CreatedOn { get; set; }

    public virtual DateTime? ModifiedOn { get; set; }

    public virtual string CreatedBy { get; set; }

    public virtual string ModifiedBy { get; set; }
}
