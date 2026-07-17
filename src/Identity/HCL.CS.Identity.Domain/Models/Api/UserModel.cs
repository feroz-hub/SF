using HCL.CS.Domain.Enums;

namespace HCL.CS.Domain.Models.Api;

public class UserModel : BaseModel
{
    public virtual string UserName { get; set; }

    public virtual string Password { get; set; }

    public virtual string FirstName { get; set; }

    public virtual string LastName { get; set; }

    public virtual DateTime? DateOfBirth { get; set; }

    public virtual string Email { get; set; }

    public virtual bool EmailConfirmed { get; set; } = false;

    public virtual string PhoneNumber { get; set; }

    public virtual bool PhoneNumberConfirmed { get; set; } = false;

    public virtual bool TwoFactorEnabled { get; set; }

    public virtual TwoFactorType TwoFactorType { get; set; } = TwoFactorType.None;

    public virtual DateTimeOffset? LockoutEnd { get; set; }

    public virtual bool LockoutEnabled { get; set; }

    public virtual int AccessFailedCount { get; set; }

    public virtual DateTime? LastPasswordChangedDate { get; set; }

    public virtual bool? RequiresDefaultPasswordChange { get; set; }

    public virtual DateTime? LastLoginDateTime { get; set; }

    public virtual DateTime? LastLogoutDateTime { get; set; }

    public virtual IdentityProvider IdentityProviderType { get; set; } = IdentityProvider.Local;

    public virtual List<UserSecurityQuestionModel> UserSecurityQuestion { get; set; }

    public virtual List<UserClaimModel> UserClaims { get; set; }
}

public class UserDisplayModel : BaseModel
{
    public virtual string UserName { get; set; }

    public virtual string Password { get; set; }

    public virtual string FirstName { get; set; }

    public virtual string LastName { get; set; }

    public virtual string Email { get; set; }

    public virtual string PhoneNumber { get; set; }

    public virtual DateTimeOffset? LockoutEnd { get; set; }

    public virtual bool LockoutEnabled { get; set; }
}
