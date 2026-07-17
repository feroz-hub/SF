using HCL.CS.Domain;
using HCL.CS.Domain.Models.Api;
using HCL.CS.Domain.Models.Api.Response;

namespace HCL.CS.Service.Interfaces.Interfaces.Api;

public partial interface IUserAccountService
{
    Task<FrameworkResult> AddUserRoleAsync(UserRoleModel userRoleModel);

    Task<FrameworkResult> AddUserRolesAsync(IList<UserRoleModel> modelList);

    Task<FrameworkResult> RemoveUserRoleAsync(UserRoleModel userRoleModel);

    Task<FrameworkResult> RemoveUserRolesAsync(IList<UserRoleModel> modelList);

    Task<IList<string>> GetUserRolesAsync(Guid userId);

    Task<IList<UserModel>> GetUsersInRoleAsync(string roleName);

    Task<UserPermissionsResponseModel> GetUserRoleClaimsByIdAsync(Guid userId);

    Task<UserPermissionsResponseModel> GetUserRoleClaimsByNameAsync(string userName);
}
