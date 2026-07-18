/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using HCL.CS.Domain.Entities.Api;

namespace HCL.CS.Infrastructure.Data.Wrappers;

public class UserStoreWrapper :
    UserStore<Users, Roles, ApplicationDbContext, Guid, UserClaims, UserRoles, UserLogins, UserTokens, RoleClaims>
{
    public UserStoreWrapper(ApplicationDbContext context) : base(context)
    {
        AutoSaveChanges = false;
    }
}
