using AutoMapper;
using HCL.CS.Domain;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.Models.Api;
using HCL.CS.DomainServices;
using HCL.CS.DomainServices.Infra;
using HCL.CS.DomainServices.Repository.Api;
using HCL.CS.DomainServices.UnitOfWork.Api;
using HCL.CS.DomainServices.Wrappers;
using HCL.CS.Service.Implementation.Api.Services;
using HCL.CS.Service.Interfaces.Interfaces.Api;
using HCL.CS.Service.Interfaces.Interfaces.Api.Wrapper;

namespace HCL.CS.ProxyService.Proxy;

public sealed class RoleProxyService : RoleService, IRoleService
{
    private readonly IApiValidator apiValidator;
    private readonly IFrameworkResultService frameworkResult;

    public RoleProxyService(
        RoleManagerWrapper<Roles> roleManager,
        ILoggerInstance logger,
        IUserRepository userRepository,
        IMapper mapper,
        IRoleManagementUnitOfWork roleUnitOfWork,
        IFrameworkResultService frameworkResultService,
        IApiResourceRepository apiResourceRepository,
        IRepository<ApiScopes> apiScopeRepository,
        IIdentityResourceRepository identityResourceRepository,
        IAuditTrailService auditTrailService,
        IApiValidator apiValidator)
        : base(
            roleManager,
            logger,
            userRepository,
            mapper,
            roleUnitOfWork,
            frameworkResultService,
            apiResourceRepository,
            apiScopeRepository,
            identityResourceRepository,
            auditTrailService)
    {
        this.apiValidator = apiValidator;
        frameworkResult = frameworkResultService;
    }

    public override async Task<FrameworkResult> CreateRoleAsync(RoleModel roleModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.CreateRoleAsync(roleModel);
    }

    public override async Task<FrameworkResult> UpdateRoleAsync(RoleModel roleModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.UpdateRoleAsync(roleModel);
    }

    public override async Task<FrameworkResult> DeleteRoleAsync(Guid roleId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.DeleteRoleAsync(roleId);
    }

    public override async Task<FrameworkResult> DeleteRoleAsync(string roleName)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.DeleteRoleAsync(roleName);
    }

    public override async Task<RoleModel> GetRoleAsync(Guid roleId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetRoleAsync(roleId);
    }

    public override async Task<RoleModel> GetRoleAsync(string roleName)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetRoleAsync(roleName);
    }

    public override async Task<IList<RoleModel>> GetAllRolesAsync()
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetAllRolesAsync();
    }

    public override async Task<FrameworkResult> AddRoleClaimAsync(RoleClaimModel roleClaimModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.AddRoleClaimAsync(roleClaimModel);
    }

    public override async Task<FrameworkResult> AddRoleClaimsAsync(IList<RoleClaimModel> roleClaimsModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.AddRoleClaimsAsync(roleClaimsModel);
    }

    public override async Task<FrameworkResult> RemoveRoleClaimAsync(int roleClaimId)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.RemoveRoleClaimAsync(roleClaimId);
    }

    public override async Task<FrameworkResult> RemoveRoleClaimAsync(RoleClaimModel roleClaimModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.RemoveRoleClaimAsync(roleClaimModel);
    }

    public override async Task<FrameworkResult> RemoveRoleClaimsAsync(IList<RoleClaimModel> roleClaimsModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed) return result;

        return await base.RemoveRoleClaimsAsync(roleClaimsModel);
    }

    public override async Task<IList<RoleClaimModel>> GetRoleClaimAsync(RoleModel roleModel)
    {
        var result = await apiValidator.ValidateRequest();
        if (result.Status == ResultStatus.Failed)
            frameworkResult.ThrowCustomMessage(result.Errors.FirstOrDefault().Description);

        return await base.GetRoleClaimAsync(roleModel);
    }
}
