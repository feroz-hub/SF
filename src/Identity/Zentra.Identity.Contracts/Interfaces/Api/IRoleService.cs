using Zentra.Domain;
using Zentra.Domain.Models.Api;

namespace Zentra.Service.Interfaces.Interfaces.Api;

public interface IRoleService
{
    Task<FrameworkResult> CreateRoleAsync(RoleModel roleModel);

    Task<FrameworkResult> UpdateRoleAsync(RoleModel roleModel);

    Task<FrameworkResult> DeleteRoleAsync(Guid roleId);

    Task<FrameworkResult> DeleteRoleAsync(string roleName);

    Task<RoleModel> GetRoleAsync(Guid roleId);

    Task<RoleModel> GetRoleAsync(string roleName);

    Task<IList<RoleModel>> GetAllRolesAsync();

    Task<FrameworkResult> AddRoleClaimAsync(RoleClaimModel roleClaimModel);

    Task<FrameworkResult> AddRoleClaimsAsync(IList<RoleClaimModel> roleClaimsModel);

    Task<IList<UserRoleClaimTypesModel>> GetRolesAndClaimsForUser(Guid userId);

    Task<FrameworkResult> RemoveRoleClaimAsync(int roleClaimId);

    Task<FrameworkResult> RemoveRoleClaimAsync(RoleClaimModel roleClaimModel);

    Task<FrameworkResult> RemoveRoleClaimsAsync(IList<RoleClaimModel> roleClaimsModel);

    Task<IList<RoleClaimModel>> GetRoleClaimAsync(RoleModel roleModel);
}
