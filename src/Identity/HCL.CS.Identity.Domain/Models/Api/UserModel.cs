/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

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

    public virtual string? DirectoryImmutableId { get; set; }

    public virtual string? EmployeeId { get; set; }

    public virtual string? UserPrincipalName { get; set; }

    public virtual string? DisplayName { get; set; }

    public virtual string? Department { get; set; }

    public virtual string? AuthenticationSource { get; set; }

    public virtual DateTimeOffset? DirectoryLastValidatedAt { get; set; }

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
