/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using AutoMapper;
using HCL.CS.Domain;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.Domain.Models.Api;
using HCL.CS.DomainServices;
using HCL.CS.DomainServices.Infra;
using HCL.CS.DomainServices.Repository.Api;
using HCL.CS.Service.Extension;
using HCL.CS.Service.Implementation.Api.Specifications;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Interfaces.Interfaces.Api;

namespace HCL.CS.Service.Implementation.Api.Services;

public class ApiResourceService(
    ILoggerInstance instance,
    IMapper mapper,
    IFrameworkResultService frameworkResult,
    IApiResourceRepository apiResourceRepository,
    IRepository<ApiResourceClaims> apiResourceClaimRepository,
    IRepository<ApiScopes> apiScopeRepository,
    IRepository<ApiScopeClaims> apiScopeClaimRepository)
    : SecurityBase, IApiResourceService
{
    private readonly ILoggerService loggerService = instance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);

    public virtual async Task<FrameworkResult> AddApiResourceAsync(ApiResourcesModel apiResourceModel)
    {
        if (apiResourceModel == null) return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.ApiResourceIsNull);

        try
        {
            var apiResourceModelSpecification = new ApiResourceModelSpecification(CrudMode.Add, apiResourceRepository);
            var apiResourceModelValidation = await apiResourceModelSpecification.ValidateAsync(apiResourceModel);
            if (apiResourceModelSpecification.IsValid)
            {
                loggerService.WriteTo(Log.Debug, "Entered into add api resource :" + apiResourceModel.Name);
                var apiResourceEntity = mapper.Map<ApiResourcesModel, ApiResources>(apiResourceModel);

                await apiResourceRepository.InsertAsync(apiResourceEntity);
                return await apiResourceRepository.SaveChangesAsync();
            }

            return frameworkResult.Failed<FrameworkResult>(apiResourceModelValidation.ErrorCode);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to add API resources.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> UpdateApiResourceAsync(ApiResourcesModel apiResourceModel)
    {
        if (apiResourceModel == null) return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.ApiResourceIsNull);

        try
        {
            var apiResourceModelSpecification =
                new ApiResourceModelSpecification(CrudMode.Update, apiResourceRepository);
            var validationError = await apiResourceModelSpecification.ValidateAsync(apiResourceModel);
            if (apiResourceModelSpecification.IsValid)
            {
                // Load only the resource row (no scopes/claims) to avoid child RowVersion concurrency conflicts.
                var apiResourceEntity = await apiResourceRepository.GetApiResourceForUpdateAsync(apiResourceModel.Id);
                if (apiResourceEntity != null)
                {
                    apiResourceEntity.Name = apiResourceModel.Name;
                    apiResourceEntity.DisplayName = apiResourceModel.DisplayName;
                    apiResourceEntity.Description = apiResourceModel.Description;
                    apiResourceEntity.Enabled = apiResourceModel.Enabled;
                    apiResourceEntity.ModifiedBy = apiResourceModel.ModifiedBy;
                    apiResourceEntity.ModifiedOn = apiResourceModel.ModifiedOn;
                    loggerService.WriteTo(Log.Debug, "Entered into update api resource :" + apiResourceEntity.Name);
                    await apiResourceRepository.UpdateAsync(apiResourceEntity);
                    return await apiResourceRepository.SaveChangesAsync();
                }

                return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.ApiResourceIdInvalid);
            }

            return frameworkResult.Failed<FrameworkResult>(validationError.ErrorCode);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed while updating API resources.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> DeleteApiResourceAsync(Guid apiResourceId)
    {
        if (!apiResourceId.IsValid())
            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.ApiResourceIdInvalid);

        try
        {
            var apiResource = await apiResourceRepository.GetApiResourcesAsync(apiResourceId);
            if (apiResource != null)
            {
                loggerService.WriteTo(Log.Debug, "Entered into remove api resource :" + apiResource.Name);
                await apiResourceRepository.DeleteAsync(apiResource);
                return await apiResourceRepository.SaveChangesAsync();
            }

            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.ApiResourceIdInvalid);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to remove API resource.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> DeleteApiResourceAsync(string apiResourceName)
    {
        if (string.IsNullOrWhiteSpace(apiResourceName))
            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.ApiResourceNameRequired);

        try
        {
            var apiResource = await apiResourceRepository.GetApiResourcesAsync(apiResourceName);
            if (apiResource != null)
            {
                loggerService.WriteTo(Log.Debug, "Entered into remove api resource :" + apiResource.Name);
                await apiResourceRepository.DeleteAsync(apiResource);
                return await apiResourceRepository.SaveChangesAsync();
            }

            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.ApiResourceNameInvalid);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to remove API resource.");
            throw;
        }
    }

    public virtual async Task<ApiResourcesModel> GetApiResourceAsync(Guid apiResourceId)
    {
        if (!apiResourceId.IsValid()) frameworkResult.Throw(ApiErrorCodes.ApiResourceIdInvalid);

        try
        {
            var apiResource = await apiResourceRepository.GetApiResourcesAsync(apiResourceId);
            if (apiResource != null)
            {
                loggerService.WriteTo(Log.Debug, "Entered into get into api resource : " + apiResource.Name);
                return mapper.Map<ApiResources, ApiResourcesModel>(apiResource);
            }

            return frameworkResult.EmptyResult<ApiResourcesModel>(ApiErrorCodes.NoRecordsFound);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to get API resources.");
            throw;
        }
    }

    public virtual async Task<ApiResourcesModel> GetApiResourceAsync(string apiResourceName)
    {
        if (string.IsNullOrWhiteSpace(apiResourceName)) frameworkResult.Throw(ApiErrorCodes.ApiResourceNameRequired);

        try
        {
            var apiResource = await apiResourceRepository.GetApiResourcesAsync(apiResourceName);
            if (apiResource != null)
            {
                loggerService.WriteTo(Log.Debug, "Entered into get into api resource : " + apiResource.Name);
                return mapper.Map<ApiResources, ApiResourcesModel>(apiResource);
            }

            return frameworkResult.EmptyResult<ApiResourcesModel>(ApiErrorCodes.NoRecordsFound);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to get API resources.");
            throw;
        }
    }

    public virtual async Task<IList<ApiResourcesModel>> GetAllApiResourcesAsync()
    {
        try
        {
            var apiResourcesEntity = await apiResourceRepository.GetAllApiResourcesAsync();
            if (apiResourcesEntity.ContainsAny())
            {
                loggerService.WriteTo(Log.Debug,
                    "Entered into get all api resources - Count :" + apiResourcesEntity.Count);
                return mapper.Map<List<ApiResources>, List<ApiResourcesModel>>(apiResourcesEntity.ToList());
            }

            return frameworkResult.EmptyResult<IList<ApiResourcesModel>>(ApiErrorCodes.NoRecordsFound);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed while retrieving API resources.");
            throw;
        }
    }

    public virtual async Task<IList<ApiResourcesByScopesModel>> GetAllApiResourcesByScopesAsync(
        IList<string> requestedScopes)
    {
        if (!requestedScopes.ContainsAny()) frameworkResult.Throw(EndpointErrorCodes.InvalidScopeOrNotAllowed);

        try
        {
            var apiResourcesEntity = await apiResourceRepository.GetAllApiResourcesByScopesAsync(requestedScopes);
            if (apiResourcesEntity.ContainsAny())
            {
                loggerService.WriteTo(Log.Debug,
                    "Entered into get all api resources - Count :" + apiResourcesEntity.Count);
                return apiResourcesEntity.ToList();
            }

            return frameworkResult.EmptyResult<IList<ApiResourcesByScopesModel>>(ApiErrorCodes.NoRecordsFound);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed while retrieving API resources.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> AddApiResourceClaimAsync(ApiResourceClaimsModel apiResourceClaimModel)
    {
        if (apiResourceClaimModel == null)
            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.ApiResourceClaimIsNull);

        try
        {
            var apiResourceClaimModelSpecification =
                new ApiResourceClaimModelSpecification(CrudMode.Add, apiResourceClaimRepository);
            var apiResourceModelValidation =
                await apiResourceClaimModelSpecification.ValidateAsync(apiResourceClaimModel);
            if (apiResourceClaimModelSpecification.IsValid)
            {
                loggerService.WriteTo(Log.Debug,
                    "Entered into add api resource claim :" + apiResourceClaimModel.ApiResourceId);
                var apiResourceClaimEntity =
                    mapper.Map<ApiResourceClaimsModel, ApiResourceClaims>(apiResourceClaimModel);

                await apiResourceClaimRepository.InsertAsync(apiResourceClaimEntity);
                return await apiResourceClaimRepository.SaveChangesAsync();
            }

            return frameworkResult.Failed<FrameworkResult>(apiResourceModelValidation.ErrorCode);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to add api resource claim.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> DeleteApiResourceClaimByResourceIdAsync(Guid apiResourceId)
    {
        if (!apiResourceId.IsValid())
            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.ApiResourceIdInvalid);

        try
        {
            var apiResource = await apiResourceClaimRepository.GetAsync(x => x.ApiResourceId == apiResourceId);
            if (apiResource.ContainsAny())
            {
                loggerService.WriteTo(Log.Debug, "Entered into remove api resource claims :" + apiResourceId);
                await apiResourceClaimRepository.DeleteAsync(apiResource);
                return await apiResourceClaimRepository.SaveChangesAsync();
            }

            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.ApiResourceIdInvalid);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to remove api resource claims.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> DeleteApiResourceClaimByIdAsync(Guid apiResourceClaimId)
    {
        if (!apiResourceClaimId.IsValid())
            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.ApiResourceClaimIdInvalid);

        try
        {
            var apiResource = await apiResourceClaimRepository.GetAsync(x => x.Id == apiResourceClaimId);
            if (apiResource.ContainsAny())
            {
                loggerService.WriteTo(Log.Debug, "Entered into remove api resource claims :" + apiResourceClaimId);
                await apiResourceClaimRepository.DeleteAsync(apiResource);
                return await apiResourceClaimRepository.SaveChangesAsync();
            }

            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.ApiResourceClaimIdInvalid);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to remove api resource claims.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> DeleteApiResourceClaimAsync(ApiResourceClaimsModel apiResourceClaimModel)
    {
        if (apiResourceClaimModel == null)
            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.ApiResourceClaimIsNull);

        try
        {
            var apiResourceClaimModelSpecification =
                new ApiResourceClaimModelSpecification(CrudMode.Delete, apiResourceClaimRepository);
            var apiResourceModelValidation =
                await apiResourceClaimModelSpecification.ValidateAsync(apiResourceClaimModel);
            if (apiResourceClaimModelSpecification.IsValid)
            {
                var apiResource = await apiResourceClaimRepository.GetAsync(x =>
                    x.ApiResourceId == apiResourceClaimModel.ApiResourceId && x.Type == apiResourceClaimModel.Type);
                if (apiResource.ContainsAny())
                {
                    loggerService.WriteTo(Log.Debug,
                        "Entered into remove api resource claim :" + apiResourceClaimModel.ApiResourceId);
                    await apiResourceClaimRepository.DeleteAsync(apiResource);
                    return await apiResourceClaimRepository.SaveChangesAsync();
                }

                return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.InvalidApiResourceClaimTypeOrResourceId);
            }

            return frameworkResult.Failed<FrameworkResult>(apiResourceModelValidation.ErrorCode);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to remove api resource claims.");
            throw;
        }
    }

    public virtual async Task<IList<ApiResourceClaimsModel>> GetApiResourceClaimsAsync(Guid apiResourceId)
    {
        if (!apiResourceId.IsValid()) frameworkResult.Throw(ApiErrorCodes.ApiResourceIdInvalid);

        try
        {
            var apiResource = await apiResourceClaimRepository.GetAsync(x => x.ApiResourceId == apiResourceId);
            if (apiResource.ContainsAny())
            {
                loggerService.WriteTo(Log.Debug, "Entered into get api resource claims - Count : " + apiResource.Count);
                return mapper.Map<IList<ApiResourceClaims>, IList<ApiResourceClaimsModel>>(apiResource);
            }

            return frameworkResult.EmptyResult<IList<ApiResourceClaimsModel>>(ApiErrorCodes.NoRecordsFound);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to get API resource claims.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> AddApiScopeAsync(ApiScopesModel apiScopesModel)
    {
        if (apiScopesModel == null) return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.ApiScopeIsNull);

        try
        {
            var apiScopeModelSpecification = new ApiScopeModelSpecification(CrudMode.Add, apiScopeRepository);
            var apiScopeModelValidation = await apiScopeModelSpecification.ValidateAsync(apiScopesModel);
            if (apiScopeModelSpecification.IsValid)
            {
                loggerService.WriteTo(Log.Debug, "Entered into add api scopes :" + apiScopesModel.Name);
                var apiScopeEntity = mapper.Map<ApiScopesModel, ApiScopes>(apiScopesModel);

                await apiScopeRepository.InsertAsync(apiScopeEntity);
                return await apiScopeRepository.SaveChangesAsync();
            }

            return frameworkResult.Failed<FrameworkResult>(apiScopeModelValidation.ErrorCode);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to add API scopes.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> UpdateApiScopeAsync(ApiScopesModel apiScopesModel)
    {
        if (apiScopesModel == null) return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.ApiScopeIsNull);

        try
        {
            var apiScopeModelSpecification = new ApiScopeModelSpecification(CrudMode.Update, apiScopeRepository);
            var apiScopeModelValidation = await apiScopeModelSpecification.ValidateAsync(apiScopesModel);
            if (apiScopeModelSpecification.IsValid)
            {
                // Load only the scope row (no claims) to avoid child RowVersion concurrency conflicts.
                var apiScopeEntity = await apiScopeRepository.GetAsync(apiScopesModel.Id);
                if (apiScopeEntity != null)
                {
                    apiScopeEntity.ApiResourceId = apiScopesModel.ApiResourceId;
                    apiScopeEntity.Name = apiScopesModel.Name;
                    apiScopeEntity.DisplayName = apiScopesModel.DisplayName;
                    apiScopeEntity.Description = apiScopesModel.Description;
                    apiScopeEntity.Required = apiScopesModel.Required;
                    apiScopeEntity.Emphasize = apiScopesModel.Emphasize;
                    apiScopeEntity.ModifiedBy = apiScopesModel.ModifiedBy;
                    apiScopeEntity.ModifiedOn = apiScopesModel.ModifiedOn;
                    loggerService.WriteTo(Log.Debug, "Entered into update api scopes :" + apiScopeEntity.Name);
                    await apiScopeRepository.UpdateAsync(apiScopeEntity);
                    return await apiScopeRepository.SaveChangesAsync();
                }

                return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.ApiScopeIdInvalid);
            }

            return frameworkResult.Failed<FrameworkResult>(apiScopeModelValidation.ErrorCode);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed while updating API scopes.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> DeleteApiScopeAsync(Guid apiScopeId)
    {
        if (!apiScopeId.IsValid()) return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.ApiScopeIdInvalid);

        try
        {
            var apiScopesList = await apiScopeRepository.GetAsync(
                x => x.Id == apiScopeId,
                x => x,
                null,
                new[] { (System.Linq.Expressions.Expression<Func<ApiScopes, object>>)(x => x.ApiScopeClaims) });
            if (apiScopesList.ContainsAny())
            {
                var apiScopes = apiScopesList.FirstOrDefault();
                loggerService.WriteTo(Log.Debug, "Entered into remove api scopes :" + apiScopes.Name);
                await apiScopeRepository.DeleteAsync(apiScopes);
                return await apiScopeRepository.SaveChangesAsync();
            }

            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.ApiScopeIdInvalid);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to remove API scopes.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> DeleteApiScopeAsync(string apiScopeName)
    {
        if (string.IsNullOrWhiteSpace(apiScopeName))
            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.ApiScopeNameRequired);

        try
        {
            var apiScopesList = await apiScopeRepository.GetAsync(
                x => x.Name == apiScopeName,
                x => x,
                null,
                new[] { (System.Linq.Expressions.Expression<Func<ApiScopes, object>>)(x => x.ApiScopeClaims) });
            if (apiScopesList.ContainsAny())
            {
                var apiScopes = apiScopesList.FirstOrDefault();
                loggerService.WriteTo(Log.Debug, "Entered into remove api scopes :" + apiScopes.Name);
                await apiScopeRepository.DeleteAsync(apiScopes);
                return await apiScopeRepository.SaveChangesAsync();
            }

            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.ApiScopeNameInvalid);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to remove API scopes.");
            throw;
        }
    }

    public virtual async Task<ApiScopesModel> GetApiScopeAsync(Guid apiScopeId)
    {
        if (!apiScopeId.IsValid()) return frameworkResult.EmptyResult<ApiScopesModel>(ApiErrorCodes.ApiScopeIdInvalid);

        try
        {
            var apiScopesList = await apiScopeRepository.GetAsync(
                x => x.Id == apiScopeId,
                x => x,
                null,
                new[] { (System.Linq.Expressions.Expression<Func<ApiScopes, object>>)(x => x.ApiScopeClaims) });
            if (apiScopesList.ContainsAny())
            {
                loggerService.WriteTo(Log.Debug, "Entered into get api scopes - Count : " + apiScopesList.Count);
                var apiScopes = apiScopesList.FirstOrDefault();
                return mapper.Map<ApiScopes, ApiScopesModel>(apiScopes);
            }

            return frameworkResult.EmptyResult<ApiScopesModel>(ApiErrorCodes.NoRecordsFound);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to get API scopes.");
            throw;
        }
    }

    public virtual async Task<ApiScopesModel> GetApiScopeAsync(string apiScopeName)
    {
        if (string.IsNullOrWhiteSpace(apiScopeName)) frameworkResult.Throw(ApiErrorCodes.ApiScopeNameRequired);

        try
        {
            var apiScopesList = await apiScopeRepository.GetAsync(
                x => x.Name == apiScopeName,
                x => x,
                null,
                new[] { (System.Linq.Expressions.Expression<Func<ApiScopes, object>>)(x => x.ApiScopeClaims) });
            if (apiScopesList.ContainsAny())
            {
                loggerService.WriteTo(Log.Debug, "Entered into get api scopes - Count : " + apiScopesList.Count);
                var apiScopes = apiScopesList.FirstOrDefault();
                return mapper.Map<ApiScopes, ApiScopesModel>(apiScopes);
            }

            return frameworkResult.EmptyResult<ApiScopesModel>(ApiErrorCodes.NoRecordsFound);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to get API scopes.");
            throw;
        }
    }

    public virtual async Task<IList<ApiScopesModel>> GetAllApiScopesAsync()
    {
        try
        {
            var apiScopesEntity = await apiResourceRepository.GetAllApiScopesAsync();
            if (apiScopesEntity.ContainsAny())
            {
                loggerService.WriteTo(Log.Debug, "Entered into get all api scopes - Count :" + apiScopesEntity.Count);
                return mapper.Map<List<ApiScopes>, List<ApiScopesModel>>(apiScopesEntity.ToList());
            }

            return frameworkResult.EmptyResult<IList<ApiScopesModel>>(ApiErrorCodes.NoRecordsFound);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed while retrieving API scopes.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> AddApiScopeClaimAsync(ApiScopeClaimsModel apiScopeClaimModel)
    {
        if (apiScopeClaimModel == null)
            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.ApiScopeClaimIsNull);

        try
        {
            var apiScopeClaimModelSpecification =
                new ApiScopeClaimModelSpecification(CrudMode.Add, apiScopeClaimRepository);
            var apiScopeClaimModelValidation = await apiScopeClaimModelSpecification.ValidateAsync(apiScopeClaimModel);
            if (apiScopeClaimModelSpecification.IsValid)
            {
                loggerService.WriteTo(Log.Debug, "Entered into add Api scope claim :" + apiScopeClaimModel.ApiScopeId);
                var apiScopeClaimEntity = mapper.Map<ApiScopeClaimsModel, ApiScopeClaims>(apiScopeClaimModel);
                await apiScopeClaimRepository.InsertAsync(apiScopeClaimEntity);
                return await apiScopeClaimRepository.SaveChangesAsync();
            }

            return frameworkResult.Failed<FrameworkResult>(apiScopeClaimModelValidation.ErrorCode);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to add API scope claim.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> DeleteApiScopeClaimByScopeIdAsync(Guid apiScopeId)
    {
        if (!apiScopeId.IsValid()) return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.ApiScopeIdInvalid);

        try
        {
            var apiScopeClaim = await apiScopeClaimRepository.GetAsync(x => x.ApiScopeId == apiScopeId);
            if (apiScopeClaim.ContainsAny())
            {
                loggerService.WriteTo(Log.Debug, "Entered into remove api scope claims :" + apiScopeId);
                await apiScopeClaimRepository.DeleteAsync(apiScopeClaim);
                return await apiScopeClaimRepository.SaveChangesAsync();
            }

            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.ApiScopeIdInvalid);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to remove API scope claim.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> DeleteApiScopeClaimByIdAsync(Guid apiScopeClaimId)
    {
        if (!apiScopeClaimId.IsValid())
            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.ApiScopeClaimIdInvalid);

        try
        {
            var apiScopeClaim = await apiScopeClaimRepository.GetAsync(x => x.Id == apiScopeClaimId);
            if (apiScopeClaim.ContainsAny())
            {
                loggerService.WriteTo(Log.Debug, "Entered into remove api scope claims by id :" + apiScopeClaimId);
                await apiScopeClaimRepository.DeleteAsync(apiScopeClaim);
                return await apiScopeClaimRepository.SaveChangesAsync();
            }

            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.ApiScopeClaimIdInvalid);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to remove API scope claim.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> DeleteApiScopeClaimAsync(ApiScopeClaimsModel apiScopeClaimModel)
    {
        if (apiScopeClaimModel == null)
            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.ApiScopeClaimIsNull);

        try
        {
            var apiScopeClaimModelSpecification =
                new ApiScopeClaimModelSpecification(CrudMode.Delete, apiScopeClaimRepository);
            var apiScopeClaimModelValidation = await apiScopeClaimModelSpecification.ValidateAsync(apiScopeClaimModel);
            if (apiScopeClaimModelSpecification.IsValid)
            {
                var apiScope = await apiScopeClaimRepository.GetAsync(x =>
                    x.ApiScopeId == apiScopeClaimModel.ApiScopeId && x.Type == apiScopeClaimModel.Type);
                if (apiScope.ContainsAny())
                {
                    loggerService.WriteTo(Log.Debug,
                        "Entered into remove api scope claim :" + apiScopeClaimModel.ApiScopeId);
                    await apiScopeClaimRepository.DeleteAsync(apiScope);
                    return await apiScopeClaimRepository.SaveChangesAsync();
                }

                return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.InvalidApiScopeClaimTypeOrScopeId);
            }

            return frameworkResult.Failed<FrameworkResult>(apiScopeClaimModelValidation.ErrorCode);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to remove API scope claim.");
            throw;
        }
    }

    public virtual async Task<IList<ApiScopeClaimsModel>> GetApiScopeClaimsAsync(Guid apiScopeId)
    {
        if (!apiScopeId.IsValid())
            return frameworkResult.EmptyResult<IList<ApiScopeClaimsModel>>(ApiErrorCodes.NoRecordsFound);

        try
        {
            var apiScope = await apiScopeClaimRepository.GetAsync(x => x.ApiScopeId == apiScopeId);
            if (apiScope.ContainsAny())
            {
                loggerService.WriteTo(Log.Debug, "Entered into get api scope claims - Count : " + apiScope.Count);
                return mapper.Map<IList<ApiScopeClaims>, IList<ApiScopeClaimsModel>>(apiScope);
            }

            return frameworkResult.EmptyResult<IList<ApiScopeClaimsModel>>(ApiErrorCodes.NoRecordsFound);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to get API resource claims.");
            throw;
        }
    }
}
