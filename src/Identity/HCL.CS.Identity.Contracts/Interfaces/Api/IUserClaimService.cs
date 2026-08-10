/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Security.Claims;
using HCL.CS.Domain;
using HCL.CS.Domain.Models.Api;

namespace HCL.CS.Service.Interfaces.Interfaces.Api;

public partial interface IUserAccountService
{
    Task<FrameworkResult> AddClaimAsync(UserClaimModel userClaimModel);

    Task<FrameworkResult> AddClaimAsync(IList<UserClaimModel> userClaimModels);

    Task<FrameworkResult> AddAdminClaimAsync(IList<UserClaimModel> userClaimModels);

    Task<FrameworkResult> AddAdminClaimAsync(UserClaimModel userClaimModel);

    Task<FrameworkResult> RemoveClaimAsync(UserClaimModel userClaimModel);

    Task<FrameworkResult> RemoveClaimAsync(IList<UserClaimModel> userClaimModel);

    Task<FrameworkResult> RemoveAdminClaimAsync(UserClaimModel userClaimModel);

    Task<FrameworkResult> RemoveAdminClaimAsync(IList<UserClaimModel> userClaimModel);

    Task<FrameworkResult> ReplaceClaimAsync(UserClaimModel existingUserClaimModel, UserClaimModel newUserClaimModel);

    Task<IList<Claim>> GetClaimsAsync(Guid userId);

    Task<IList<UserClaimModel>> GetUserClaimsAsync(Guid userId);

    Task<IList<UserClaimModel>> GetAdminUserClaimsAsync(Guid userId);
}
