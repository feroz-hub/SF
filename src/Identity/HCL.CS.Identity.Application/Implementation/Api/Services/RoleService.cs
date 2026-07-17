using System.Security.Claims;
using AutoMapper;
using Newtonsoft.Json;
using HCL.CS.Domain;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.Enums;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.Domain.Models.Api;
using HCL.CS.DomainServices;
using HCL.CS.DomainServices.Infra;
using HCL.CS.DomainServices.Repository.Api;
using HCL.CS.DomainServices.UnitOfWork.Api;
using HCL.CS.DomainServices.Wrappers;
using HCL.CS.Service.Extension;
using HCL.CS.Service.Implementation.Api.Extension;
using HCL.CS.Service.Implementation.Api.Specifications;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Interfaces.Interfaces.Api;

namespace HCL.CS.Service.Implementation.Api.Services;

public class RoleService(
    RoleManagerWrapper<Roles> roleManager,
    ILoggerInstance logger,
    IUserRepository userRepository,
    IMapper mapper,
    IRoleManagementUnitOfWork roleUnitOfWork,
    IFrameworkResultService frameworkResultService,
    IApiResourceRepository apiResourceRepository,
    IRepository<ApiScopes> apiScopeRepository,
    IIdentityResourceRepository identityResourceRepository,
    IAuditTrailService auditTrailService)
    : SecurityBase, IRoleService
{
    private readonly IApiResourceRepository apiResourceRepository = apiResourceRepository;
    private readonly IIdentityResourceRepository identityResourceRepository = identityResourceRepository;
    private readonly ILoggerService loggerService = logger.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);

    public virtual async Task<FrameworkResult> CreateRoleAsync(RoleModel roleModel)
    {
        if (roleModel == null) return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.RoleIsNull);

        try
        {
            // Role name condition checked here itself instead of identity, Because to check duplicate role, identity throws error on duplicate role.
            var modelSpecification = new RoleModelSpecification(CrudMode.Add, apiScopeRepository);
            var validationError = await modelSpecification.ValidateAsync(roleModel);
            if (modelSpecification.IsValid)
            {
                loggerService.WriteTo(Log.Debug, "Entered in add role : " + roleModel.Name);
                var errorCode = await IsRoleExists(roleModel.Name);
                if (string.IsNullOrWhiteSpace(errorCode))
                {
                    var entity = mapper.Map<RoleModel, Roles>(roleModel);
                    var utcNow = DateTime.UtcNow;
                    var createdBy = roleModel.CreatedBy ?? "System";
                    entity.IsDeleted = false;
                    entity.CreatedOn = roleModel.CreatedOn != default ? roleModel.CreatedOn : utcNow;
                    entity.CreatedBy = createdBy;
                    if (string.IsNullOrEmpty(entity.ConcurrencyStamp))
                        entity.ConcurrencyStamp = Guid.NewGuid().ToString();

                    var identityResult = await roleManager.CreateAsync(entity);
                    if (identityResult.Succeeded)
                    {
                        await roleUnitOfWork.SetAddedStatusAsync(entity);
                        if (roleModel.RoleClaims.ContainsAny())
                        {
                            var roleClaimEntity =
                                mapper.Map<List<RoleClaimModel>, List<RoleClaims>>(roleModel.RoleClaims);
                            foreach (var roleClaim in roleClaimEntity)
                            {
                                roleClaim.RoleId = entity.Id;
                                roleClaim.Id = 0;
                                roleClaim.IsDeleted = false;
                                roleClaim.CreatedOn = utcNow;
                                roleClaim.CreatedBy = createdBy;
                                await roleUnitOfWork.RoleClaimsRepository.InsertAsync(roleClaim);
                            }
                        }

                        var createResult = await roleUnitOfWork.SaveChangesAsync();
                        if (createResult.Status == ResultStatus.Succeeded)
                            await TryAddAuditAsync(AuditType.Create, Constants.RoleTable, null, roleModel.CreatedBy ?? "", JsonConvert.SerializeObject(roleModel));
                        return createResult;
                    }

                    return frameworkResultService.Failed(identityResult.ConstructIdentityErrorAsList());
                }

                return frameworkResultService.Failed<FrameworkResult>(errorCode);
            }

            return frameworkResultService.Failed<FrameworkResult>(validationError.ErrorCode);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to create role.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> UpdateRoleAsync(RoleModel roleModel)
    {
        if (roleModel == null) return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.RoleIsNull);

        try
        {
            var modelSpecification = new RoleModelSpecification(CrudMode.Update, apiScopeRepository);
            var validationError = await modelSpecification.ValidateAsync(roleModel);
            if (modelSpecification.IsValid)
            {
                loggerService.WriteTo(Log.Debug, "Entered in update role : " + roleModel.Name);
                var entity = await roleManager.FindByIdAsync(roleModel.Id.ToString());
                if (entity != null)
                {
                    var concurrencyStamp = entity.ConcurrencyStamp;
                    var oldValue = JsonConvert.SerializeObject(mapper.Map<Roles, RoleModel>(entity));
                    entity = mapper.Map(roleModel, entity);
                    var identityResult = await roleManager.UpdateAsync(entity);
                    if (!identityResult.Succeeded)
                        return frameworkResultService.Failed(identityResult.ConstructIdentityErrorAsList());

                    await roleUnitOfWork.SetModifiedStatusAsync(entity, concurrencyStamp);
                    if (roleModel.RoleClaims.ContainsAny())
                        foreach (var roleClaimModel in roleModel.RoleClaims)
                        {
                            var roleclaim =
                                await roleUnitOfWork.RoleClaimsRepository.FindRoleClaimByIdAsync(roleClaimModel.Id);
                            if (roleclaim != null)
                            {
                                await roleUnitOfWork.RoleClaimsRepository.DeleteAsync(roleclaim);
                                var roleClaimEntity = new RoleClaims
                                {
                                    ClaimType = roleClaimModel.ClaimType,
                                    ClaimValue = roleClaimModel.ClaimValue,
                                    CreatedBy = roleClaimModel.CreatedBy,
                                    CreatedOn = roleClaimModel.CreatedOn,
                                    ModifiedBy = roleClaimModel.ModifiedBy,
                                    RoleId = roleClaimModel.RoleId
                                };

                                await roleUnitOfWork.RoleClaimsRepository.InsertAsync(roleClaimEntity);
                            }
                            else
                            {
                                return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidRoleClaim);
                            }
                        }

                    var updateResult = await roleUnitOfWork.SaveChangesAsync();
                    if (updateResult.Status == ResultStatus.Succeeded)
                        await TryAddAuditAsync(AuditType.Update, Constants.RoleTable, oldValue, roleModel.ModifiedBy ?? roleModel.CreatedBy ?? "", JsonConvert.SerializeObject(roleModel));
                    return updateResult;
                }

                return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidRoleId);
            }

            return frameworkResultService.Failed<FrameworkResult>(validationError.ErrorCode);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to update role.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> DeleteRoleAsync(Guid roleId)
    {
        if (!roleId.IsValid()) return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.RoleIdRequired);

        try
        {
            var roleEntity = await roleManager.FindByIdAsync(roleId.ToString());
            if (roleEntity != null)
            {
                loggerService.WriteTo(Log.Debug, "Entered in delete role : " + roleEntity.Name);
                return await DeleteRoleAsync(roleEntity);
            }

            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidRoleId);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to remove role.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> DeleteRoleAsync(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.RoleNameRequired);

        try
        {
            var roleEntity = await roleManager.FindByNameAsync(roleName);
            if (roleEntity != null)
            {
                loggerService.WriteTo(Log.Debug, "Entered in delete role : " + roleEntity.Name);
                return await DeleteRoleAsync(roleEntity);
            }

            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidRoleName);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to remove role.");
            throw;
        }
    }

    public virtual async Task<RoleModel> GetRoleAsync(Guid roleId)
    {
        if (!roleId.IsValid()) frameworkResultService.Throw(ApiErrorCodes.RoleIdRequired);

        try
        {
            var role = await roleManager.FindByIdAsync(roleId.ToString());
            if (role != null)
            {
                loggerService.WriteTo(Log.Debug, "Entered in get role by id : " + roleId);
                var roleModel = mapper.Map<Roles, RoleModel>(role);

                var roleClaimsEntityList = await roleUnitOfWork.RoleClaimsRepository.GetClaimsAsync(roleModel.Id);
                if (roleClaimsEntityList.ContainsAny())
                {
                    var roleClaim = mapper.Map<IList<RoleClaims>, IList<RoleClaimModel>>(roleClaimsEntityList);
                    roleModel.RoleClaims = (List<RoleClaimModel>)roleClaim;
                }

                return roleModel;
            }

            return frameworkResultService.EmptyResult<RoleModel>(ApiErrorCodes.NoRecordsFound);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to retrieve role.");
            throw;
        }
    }

    public virtual async Task<RoleModel> GetRoleAsync(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName)) frameworkResultService.Throw(ApiErrorCodes.RoleNameRequired);

        try
        {
            loggerService.WriteTo(Log.Debug, "Entered in get role by name : " + roleName);
            var role = await roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                var roleModel = mapper.Map<Roles, RoleModel>(role);

                var roleEntity = await roleUnitOfWork.RoleClaimsRepository.GetClaimsAsync(roleModel.Id);
                if (roleEntity.ContainsAny())
                {
                    var roleClaim = mapper.Map<IList<RoleClaims>, IList<RoleClaimModel>>(roleEntity);
                    roleModel.RoleClaims = (List<RoleClaimModel>)roleClaim;
                }

                return roleModel;
            }

            return frameworkResultService.EmptyResult<RoleModel>(ApiErrorCodes.NoRecordsFound);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to retrieve role.");
            throw;
        }
    }

    public virtual async Task<IList<RoleModel>> GetAllRolesAsync()
    {
        try
        {
            var roles = await roleUnitOfWork.RoleRepository.GetAllRolesAsync();
            if (roles != null)
            {
                loggerService.WriteTo(Log.Debug, "Entered in Get all roles");
                var roleModel = mapper.Map<IList<Roles>, IList<RoleModel>>(roles);
                return roleModel;
            }

            return frameworkResultService.EmptyResult<IList<RoleModel>>(ApiErrorCodes.NoRecordsFound);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to retrieve role.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> AddRoleClaimAsync(RoleClaimModel roleClaimModel)
    {
        if (roleClaimModel == null)
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidRoleClaim);

        try
        {
            var roleClaimModelSpecification = new RoleClaimModelSpecification(apiScopeRepository);
            var roleClaimModelValidation = await roleClaimModelSpecification.ValidateAsync(roleClaimModel);
            if (roleClaimModelSpecification.IsValid)
            {
                roleClaimModel.Id = 0;
                loggerService.WriteTo(Log.Debug, "Entered in add role claims :" + roleClaimModel.Id);
                var roleClaim = new Claim(roleClaimModel.ClaimType, roleClaimModel.ClaimValue);
                var existingRoleClaim =
                    await roleUnitOfWork.RoleClaimsRepository.FindIdByClaimAsync(roleClaimModel.RoleId, roleClaim);
                if (existingRoleClaim <= 0)
                {
                    var roleClaimEntity = mapper.Map<RoleClaimModel, RoleClaims>(roleClaimModel);
                    await roleUnitOfWork.RoleClaimsRepository.InsertAsync(roleClaimEntity);
                    return await roleUnitOfWork.RoleClaimsRepository.SaveChangesAsync();
                }

                return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.RoleClaimExists);
            }

            return frameworkResultService.Failed<FrameworkResult>(roleClaimModelValidation.ErrorCode);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to add role claims.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> AddRoleClaimsAsync(IList<RoleClaimModel> roleClaimsModel)
    {
        if (!roleClaimsModel.ContainsAny())
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidRoleClaim);

        try
        {
            loggerService.WriteTo(Log.Debug, "Entered in add role claims - Count : " + roleClaimsModel.Count);
            var roleClaimModelSpecification = new RoleClaimModelSpecification(apiScopeRepository);
            var roleClaimModelValidation = await roleClaimModelSpecification.ValidateAsync(roleClaimsModel);
            if (roleClaimModelSpecification.IsValid)
            {
                var roleClaimEntity = mapper.Map<IList<RoleClaimModel>, IList<RoleClaims>>(roleClaimsModel);
                foreach (var roleClaim in roleClaimEntity)
                {
                    roleClaim.Id = 0;
                    var claim = new Claim(roleClaim.ClaimType, roleClaim.ClaimValue);
                    var existingRoleClaim =
                        await roleUnitOfWork.RoleClaimsRepository.FindIdByClaimAsync(roleClaim.RoleId, claim);
                    if (existingRoleClaim <= 0)
                        await roleUnitOfWork.RoleClaimsRepository.InsertAsync(roleClaim);
                    else
                        return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.RoleClaimExists);
                }

                return await roleUnitOfWork.RoleClaimsRepository.SaveChangesAsync();
            }

            return frameworkResultService.Failed(roleClaimModelValidation);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to add role claims.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> RemoveRoleClaimAsync(int roleClaimId)
    {
        if (roleClaimId <= 0) return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidRoleClaimId);

        try
        {
            var roleClaims = await roleUnitOfWork.RoleClaimsRepository.FindRoleClaimByIdAsync(roleClaimId);
            if (roleClaims != null)
            {
                await roleUnitOfWork.RoleClaimsRepository.DeleteAsync(roleClaims);
                return await roleUnitOfWork.RoleClaimsRepository.SaveChangesAsync();
            }

            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidRoleClaimId);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to remove role claims.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> RemoveRoleClaimAsync(RoleClaimModel roleClaimModel)
    {
        try
        {
            var roleClaimModelSpecification = new RoleClaimModelSpecification(apiScopeRepository);
            var roleClaimModelValidation = await roleClaimModelSpecification.ValidateAsync(roleClaimModel);
            if (roleClaimModelSpecification.IsValid)
            {
                var roleClaim = new Claim(roleClaimModel.ClaimType, roleClaimModel.ClaimValue);
                loggerService.WriteTo(Log.Debug, "Entered in remove role claims : " + roleClaimModel.RoleId);
                var roleClaimId =
                    await roleUnitOfWork.RoleClaimsRepository.FindIdByClaimAsync(roleClaimModel.RoleId, roleClaim);
                if (roleClaimId > 0)
                {
                    await roleUnitOfWork.RoleClaimsRepository.DeleteAsync(roleClaimId);
                    return await roleUnitOfWork.RoleClaimsRepository.SaveChangesAsync();
                }

                return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.RoleClaimNotExists);
            }

            return frameworkResultService.Failed<FrameworkResult>(roleClaimModelValidation.ErrorCode);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to remove role claims.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> RemoveRoleClaimsAsync(IList<RoleClaimModel> roleClaimsModel)
    {
        if (!roleClaimsModel.ContainsAny())
            return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.InvalidRoleClaim);

        try
        {
            var roleClaimModelSpecification = new RoleClaimModelSpecification(apiScopeRepository);
            var roleClaimModelValidation = await roleClaimModelSpecification.ValidateAsync(roleClaimsModel);
            if (roleClaimModelSpecification.IsValid)
            {
                loggerService.WriteTo(Log.Debug, "Entered in remove role claims - Count : " + roleClaimsModel.Count);
                foreach (var roleClaimModel in roleClaimsModel)
                {
                    var roleClaim = new Claim(roleClaimModel.ClaimType, roleClaimModel.ClaimValue);
                    var roleClaimId =
                        await roleUnitOfWork.RoleClaimsRepository.FindIdByClaimAsync(roleClaimModel.RoleId, roleClaim);
                    if (roleClaimId > 0)
                        await roleUnitOfWork.RoleClaimsRepository.DeleteAsync(roleClaimId);
                    else
                        return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.RoleClaimNotExists);
                }

                return await roleUnitOfWork.RoleClaimsRepository.SaveChangesAsync();
            }

            return frameworkResultService.Failed(roleClaimModelValidation);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to remove role claims.");
            throw;
        }
    }

    public virtual async Task<IList<RoleClaimModel>> GetRoleClaimAsync(RoleModel roleModel)
    {
        var roleClaims = new List<RoleClaimModel>();
        if (roleModel == null) frameworkResultService.Throw(ApiErrorCodes.RoleIsNull);

        try
        {
            var role = mapper.Map<RoleModel, Roles>(roleModel);
            var claimList = (List<Claim>)await roleManager.GetClaimsAsync(role);
            if (claimList.ContainsAny())
            {
                foreach (var item in claimList)
                    roleClaims.Add(new RoleClaimModel
                        { ClaimType = item.Type, ClaimValue = item.Value, RoleId = role.Id });

                loggerService.WriteTo(Log.Debug, "Entered in get role claims - Count : " + claimList.Count);
                return roleClaims;
            }

            return frameworkResultService.EmptyResult<IList<RoleClaimModel>>(ApiErrorCodes.NoRecordsFound);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to retrieve role claims.");
            throw;
        }
    }

    public virtual async Task<IList<UserRoleClaimTypesModel>> GetRolesAndClaimsForUser(Guid userId)
    {
        if (!userId.IsValid()) frameworkResultService.Throw(ApiErrorCodes.InvalidUserId);

        try
        {
            var userRoleClaims = await roleUnitOfWork.RoleClaimsRepository.GetRolesAndClaimsForUser(userId);
            if (userRoleClaims.ContainsAny())
            {
                loggerService.WriteTo(Log.Debug,
                    "Entered into get roles and claims for user - Count :" + userRoleClaims.Count);
                return userRoleClaims.ToList();
            }

            return frameworkResultService.EmptyResult<IList<UserRoleClaimTypesModel>>(ApiErrorCodes.NoRecordsFound);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to retrieve all user role claims");
            throw;
        }
    }

    private async Task<string> IsRoleExists(string roleName)
    {
        var duplicateRoleExists = await roleUnitOfWork.RoleRepository.ExistsByNameIncludingDeletedAsync(roleName);
        if (duplicateRoleExists) return ApiErrorCodes.RoleAlreadyExists;

        return string.Empty;
    }

    private async Task<FrameworkResult> DeleteRoleAsync(Roles roleEntity)
    {
        var roleExistsInUsers = await GetUsersInRoleAsync(roleEntity.Name);
        if (!roleExistsInUsers)
        {
            var roleModel = mapper.Map<Roles, RoleModel>(roleEntity);
            var oldValue = JsonConvert.SerializeObject(roleModel);
            await roleUnitOfWork.RoleClaimsRepository.DeleteAsync(roleEntity.Id);
            await roleUnitOfWork.RoleRepository.DeleteAsync(roleEntity);
            var deleteResult = await roleUnitOfWork.SaveChangesAsync();
            if (deleteResult.Status == ResultStatus.Succeeded)
                await TryAddAuditAsync(AuditType.Delete, Constants.RoleTable, oldValue, roleEntity.CreatedBy ?? "", null);
            return deleteResult;
        }

        return frameworkResultService.Failed<FrameworkResult>(ApiErrorCodes.RoleHasUsers);
    }

    private async Task TryAddAuditAsync(AuditType actionType, string tableName, string oldValue, string createdBy, string newValue)
    {
        try
        {
            await auditTrailService.AddAuditTrailAsync(new AuditTrailModel
            {
                ActionType = actionType,
                TableName = tableName,
                OldValue = oldValue,
                NewValue = newValue,
                CreatedBy = createdBy ?? "",
                CreatedOn = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to add audit trail.");
        }
    }

    private async Task<bool> GetUsersInRoleAsync(string roleName)
    {
        var userList = await userRepository.GetUsersInRoleAsync(roleName);
        if (userList.ContainsAny()) return true;

        return false;
    }
}
