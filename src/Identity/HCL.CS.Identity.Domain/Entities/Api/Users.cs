using Microsoft.AspNetCore.Identity;
using HCL.CS.Domain.Enums;

namespace HCL.CS.Domain.Entities.Api;

public class Users : IdentityUser<Guid>
{
    public virtual string FirstName { get; set; }

    public virtual string LastName { get; set; }

    public virtual DateTime? DateOfBirth { get; set; }

    public virtual TwoFactorType TwoFactorType { get; set; }

    public virtual DateTime? LastPasswordChangedDate { get; set; }

    public virtual bool? RequiresDefaultPasswordChange { get; set; }

    public virtual DateTime? LastLoginDateTime { get; set; }

    public virtual DateTime? LastLogoutDateTime { get; set; }

    public virtual IdentityProvider IdentityProviderType { get; set; }

    public virtual bool IsDeleted { get; set; }

    public virtual DateTime CreatedOn { get; set; }

    public virtual DateTime? ModifiedOn { get; set; }

    public virtual string CreatedBy { get; set; }

    public virtual string ModifiedBy { get; set; }
}
