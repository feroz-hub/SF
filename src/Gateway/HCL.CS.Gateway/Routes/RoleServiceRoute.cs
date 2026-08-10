/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain.Models.Api;
using HCL.CS.ProxyService.Routes.Extension;
using HCL.CS.Service.Interfaces.Interfaces.Api.Wrapper;

namespace HCL.CS.ProxyService.Routes;

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
