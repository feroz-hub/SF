using Zentra.Domain;
using Zentra.Domain.Models.Api;
using Zentra.Domain.Models.Api.Response;

namespace Zentra.Service.Interfaces.Interfaces.Api;

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
