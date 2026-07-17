using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace HCL.CS.Domain.Entities.Api;

public class UserRoles : IdentityUserRole<Guid>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public virtual Guid Id { get; set; }

    public virtual DateTime? ValidFrom { get; set; }

    public virtual DateTime? ValidTo { get; set; }

    public virtual bool IsDeleted { get; set; }

    public virtual DateTime CreatedOn { get; set; }

    public virtual DateTime? ModifiedOn { get; set; }

    public virtual string CreatedBy { get; set; }

    public virtual string ModifiedBy { get; set; }

    [Timestamp] public byte[] RowVersion { get; set; }
}
