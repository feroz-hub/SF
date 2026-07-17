using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Zentra.Domain.Entities.Api;

public class RoleClaims : IdentityRoleClaim<Guid>
{
    public virtual bool IsDeleted { get; set; }

    public virtual DateTime CreatedOn { get; set; }

    public virtual DateTime? ModifiedOn { get; set; }

    public virtual string CreatedBy { get; set; }

    public virtual string ModifiedBy { get; set; }

    [Timestamp] public byte[] RowVersion { get; set; }
}
