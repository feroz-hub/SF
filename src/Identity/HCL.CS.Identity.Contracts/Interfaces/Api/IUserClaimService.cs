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
