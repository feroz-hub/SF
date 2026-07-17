using Zentra.Domain.Models.Api;
using Zentra.ProxyService.Routes.Extension;
using Zentra.Service.Interfaces.Interfaces.Api.Wrapper;

namespace Zentra.ProxyService.Routes;

internal partial class ApiGateway : BaseApiServiceInstance, IApiGateway
{
    private async Task<bool> CreateRole(string jsonContent)
    {
        var roleModel = jsonContent.JsonDeserialize<RoleModel>();
        var frameworkResult = await RoleService.CreateRoleAsync(roleModel);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> UpdateRoleAsync(string jsonContent)
    {
        var roleModel = jsonContent.JsonDeserialize<RoleModel>();
        var frameworkResult = await RoleService.UpdateRoleAsync(roleModel);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> DeleteRoleById(string jsonContent)
    {
        var roleId = jsonContent.JsonDeserialize<Guid>();
        var frameworkResult = await RoleService.DeleteRoleAsync(roleId);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> DeleteRoleByName(string jsonContent)
    {
        var roleName = jsonContent.JsonDeserialize<string>();
        var frameworkResult = await RoleService.DeleteRoleAsync(roleName);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> GetRoleById(string jsonContent)
    {
        var roleId = jsonContent.JsonDeserialize<Guid>();
        var frameworkResult = await RoleService.GetRoleAsync(roleId);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> GetRoleByName(string jsonContent)
    {
        var roleName = jsonContent.JsonDeserialize<string>();
        var frameworkResult = await RoleService.GetRoleAsync(roleName);
        await GenerateApiResults(frameworkResult);
        return true;
    }

    private async Task<bool> GetAllRoles(string jsonContent)
    {
        var roleList = await RoleService.GetAllRolesAsync();
        await GenerateApiResults(roleList);
        return true;
    }

    private async Task<bool> AddRoleClaim(string jsonContent)
    {
        var roleClaimModel = jsonContent.JsonDeserialize<RoleClaimModel>();
        var frameworkResult = await RoleService.AddRoleClaimAsync(roleClaimModel);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> AddRoleClaimList(string jsonContent)
    {
        var roleClaimModel = jsonContent.JsonDeserialize<IList<RoleClaimModel>>();
        var frameworkResult = await RoleService.AddRoleClaimsAsync(roleClaimModel);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> RemoveRoleClaimById(string jsonContent)
    {
        var roleClaimId = jsonContent.JsonDeserialize<int>();
        var frameworkResult = await RoleService.RemoveRoleClaimAsync(roleClaimId);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> RemoveRoleClaim(string jsonContent)
    {
        var roleClaimModel = jsonContent.JsonDeserialize<RoleClaimModel>();
        var frameworkResult = await RoleService.RemoveRoleClaimAsync(roleClaimModel);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> RemoveRoleClaimsList(string jsonContent)
    {
        var roleClaimModel = jsonContent.JsonDeserialize<List<RoleClaimModel>>();
        var frameworkResult = await RoleService.RemoveRoleClaimsAsync(roleClaimModel);
        return await GenerateApiResults(frameworkResult);
    }

    private async Task<bool> GetRoleClaim(string jsonContent)
    {
        var roleModel = jsonContent.JsonDeserialize<RoleModel>();
        var frameworkResult = await RoleService.GetRoleClaimAsync(roleModel);
        await GenerateApiResults(frameworkResult);
        return true;
    }
}
