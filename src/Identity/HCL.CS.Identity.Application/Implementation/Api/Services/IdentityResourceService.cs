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

public class IdentityResourceService(
    ILoggerInstance instance,
    IMapper mapper,
    IFrameworkResultService frameworkResult,
    IIdentityResourceRepository identityResourceRepository,
    IRepository<IdentityClaims> identityClaimRepository)
    : SecurityBase, IIdentityResourceService
{
    private readonly ILoggerService loggerService = instance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);

    public virtual async Task<FrameworkResult> AddIdentityResourceAsync(IdentityResourcesModel identityResourceModel)
    {
        if (identityResourceModel == null)
            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.IdentityResourceIsNull);

        try
        {
            var identityResourceModelValidation =
                new IdentityResourceModelSpecification(CrudMode.Add, identityResourceRepository);
            var validationError = await identityResourceModelValidation.ValidateAsync(identityResourceModel);
            if (identityResourceModelValidation.IsValid)
            {
                loggerService.WriteTo(Log.Debug, "Entered into add identity resource :" + identityResourceModel.Name);
                var identityModelEntity =
                    mapper.Map<IdentityResourcesModel, IdentityResources>(identityResourceModel);

                await identityResourceRepository.InsertAsync(identityModelEntity);
                return await identityResourceRepository.SaveChangesAsync();
            }

            return frameworkResult.Failed<FrameworkResult>(validationError.ErrorCode);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to add Identity resources.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> UpdateIdentityResourceAsync(IdentityResourcesModel identityResourceModel)
    {
        if (identityResourceModel == null)
            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.IdentityResourceIsNull);

        try
        {
            var identityResourceModelValidation =
                new IdentityResourceModelSpecification(CrudMode.Update, identityResourceRepository);
            var validationError = await identityResourceModelValidation.ValidateAsync(identityResourceModel);
            if (identityResourceModelValidation.IsValid)
            {
                var identityResourceEntity =
                    await identityResourceRepository.GetIdentityResourcesAsync(identityResourceModel.Id);
                if (identityResourceEntity != null)
                {
                    identityResourceEntity = mapper.Map(identityResourceModel, identityResourceEntity);
                    loggerService.WriteTo(Log.Debug,
                        "Entered into update identity resource :" + identityResourceEntity.Name);
                    await identityResourceRepository.UpdateAsync(identityResourceEntity);
                    return await identityResourceRepository.SaveChangesAsync();
                }

                return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.IdentityResourceIdInvalid);
            }

            return frameworkResult.Failed<FrameworkResult>(validationError.ErrorCode);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to update Identity resources.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> DeleteIdentityResourceAsync(Guid identityResourceId)
    {
        try
        {
            var identityResource = await identityResourceRepository.GetIdentityResourcesAsync(identityResourceId);
            if (identityResource != null)
            {
                loggerService.WriteTo(Log.Debug, "Entered into remove identity resource :" + identityResource.Name);
                await identityResourceRepository.DeleteAsync(identityResource);
                return await identityResourceRepository.SaveChangesAsync();
            }

            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.IdentityResourceIdInvalid);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to delete Identity resources using Id.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> DeleteIdentityResourceAsync(string identityResourceName)
    {
        try
        {
            var identityResource = await identityResourceRepository.GetIdentityResourcesAsync(identityResourceName);
            if (identityResource != null)
            {
                loggerService.WriteTo(Log.Debug, "Entered into remove identity resource :" + identityResource.Name);
                await identityResourceRepository.DeleteAsync(identityResource);
                return await identityResourceRepository.SaveChangesAsync();
            }

            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.IdentityResourceNameRequired);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to delete Identity resources using Id.");
            throw;
        }
    }

    public virtual async Task<IdentityResourcesModel> GetIdentityResourceAsync(Guid identityResourceId)
    {
        try
        {
            var identityResource = await identityResourceRepository.GetIdentityResourcesAsync(identityResourceId);
            if (identityResource != null)
            {
                loggerService.WriteTo(Log.Debug, "Entered into get identity resource :" + identityResourceId);
                return mapper.Map<IdentityResources, IdentityResourcesModel>(identityResource);
            }

            return frameworkResult.EmptyResult<IdentityResourcesModel>(ApiErrorCodes.NoRecordsFound);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to retrieve Identity resources using Id.");
            throw;
        }
    }

    public virtual async Task<IdentityResourcesModel> GetIdentityResourceAsync(string identityResourceName)
    {
        try
        {
            var identityResource = await identityResourceRepository.GetIdentityResourcesAsync(identityResourceName);
            if (identityResource != null)
            {
                loggerService.WriteTo(Log.Debug, "Entered into get identity resource :" + identityResourceName);
                return mapper.Map<IdentityResources, IdentityResourcesModel>(identityResource);
            }

            return frameworkResult.EmptyResult<IdentityResourcesModel>(ApiErrorCodes.NoRecordsFound);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to retrieve Identity resources using Name.");
            throw;
        }
    }

    public virtual async Task<IList<IdentityResourcesModel>> GetAllIdentityResourcesAsync()
    {
        try
        {
            var identityResourcesEntity = await identityResourceRepository.GetAllAsync(new System.Linq.Expressions.Expression<Func<IdentityResources, object>>[] { x => x.IdentityClaims });
            if (identityResourcesEntity.ContainsAny())
            {
                loggerService.WriteTo(Log.Debug,
                    "Entered into get all identity resources - Count :" + identityResourcesEntity.Count);
                return mapper.Map<List<IdentityResources>, List<IdentityResourcesModel>>(
                    identityResourcesEntity.ToList());
            }

            return frameworkResult.EmptyResult<IList<IdentityResourcesModel>>(ApiErrorCodes.NoRecordsFound);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to retrieve all Identity resources.");
            throw;
        }
    }

    public virtual async Task<IList<IdentityResourcesByScopesModel>> GetAllIdentityResourcesByScopesAsync(
        IList<string> requestedScopes)
    {
        if (!requestedScopes.ContainsAny()) frameworkResult.Throw(EndpointErrorCodes.InvalidScopeOrNotAllowed);

        try
        {
            var identityResourcesEntity =
                await identityResourceRepository.GetAllIdentityResourcesByScopesAsync(requestedScopes);
            if (identityResourcesEntity.ContainsAny())
            {
                loggerService.WriteTo(Log.Debug,
                    "Entered into get identity resources by scopes - Count :" + identityResourcesEntity.Count);
                return identityResourcesEntity.ToList();
            }

            return frameworkResult.EmptyResult<IList<IdentityResourcesByScopesModel>>(ApiErrorCodes.NoRecordsFound);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to retrieve all Identity resources.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> AddIdentityResourceClaimAsync(IdentityClaimsModel identityClaimsModel)
    {
        if (identityClaimsModel == null)
            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.IdentityClaimIsNull);

        try
        {
            var identityClaimModelSpecification =
                new IdentityClaimModelSpecification(CrudMode.Add, identityClaimRepository);
            var identityResourceModelValidation =
                await identityClaimModelSpecification.ValidateAsync(identityClaimsModel);
            if (identityClaimModelSpecification.IsValid)
            {
                loggerService.WriteTo(Log.Debug,
                    "Entered into add identity resource claim :" + identityClaimsModel.IdentityResourceId);
                var identityResourceClaimEntity = mapper.Map<IdentityClaimsModel, IdentityClaims>(identityClaimsModel);

                await identityClaimRepository.InsertAsync(identityResourceClaimEntity);
                return await identityClaimRepository.SaveChangesAsync();
            }

            return frameworkResult.Failed<FrameworkResult>(identityResourceModelValidation.ErrorCode);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to add identity resource claim.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> DeleteIdentityResourceClaimByResourceIdAsync(Guid identityResourceId)
    {
        if (!identityResourceId.IsValid())
            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.IdentityResourceIdInvalid);

        try
        {
            var identityResource =
                await identityClaimRepository.GetAsync(x => x.IdentityResourceId == identityResourceId);
            if (identityResource.ContainsAny())
            {
                loggerService.WriteTo(Log.Debug, "Entered into remove identity resource claim :" + identityResourceId);
                await identityClaimRepository.DeleteAsync(identityResource);
                return await identityClaimRepository.SaveChangesAsync();
            }

            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.IdentityResourceIdInvalid);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to remove identity resource claims.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> DeleteIdentityResourceClaimByIdAsync(Guid identityResourceClaimId)
    {
        if (!identityResourceClaimId.IsValid())
            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.IdentityResourceClaimIdInvalid);

        try
        {
            var identityResourceClaims = await identityClaimRepository.GetAsync(x => x.Id == identityResourceClaimId);
            if (identityResourceClaims.ContainsAny())
            {
                loggerService.WriteTo(Log.Debug,
                    "Entered into remove identity resource claim :" + identityResourceClaimId);
                await identityClaimRepository.DeleteAsync(identityResourceClaims);
                return await identityClaimRepository.SaveChangesAsync();
            }

            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.IdentityResourceClaimIdInvalid);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to remove identity resource claims.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> DeleteIdentityResourceClaimAsync(IdentityClaimsModel identityClaimsModel)
    {
        if (identityClaimsModel == null)
            return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.IdentityClaimIsNull);

        try
        {
            var identityClaimModelSpecification =
                new IdentityClaimModelSpecification(CrudMode.Delete, identityClaimRepository);
            var identityResourceModelValidation =
                await identityClaimModelSpecification.ValidateAsync(identityClaimsModel);
            if (identityClaimModelSpecification.IsValid)
            {
                var identityResource = await identityClaimRepository.GetAsync(x =>
                    x.IdentityResourceId == identityClaimsModel.IdentityResourceId &&
                    x.Type == identityClaimsModel.Type);
                if (identityResource.ContainsAny())
                {
                    loggerService.WriteTo(Log.Debug,
                        "Entered into remove identity resource claim :" + identityClaimsModel.IdentityResourceId);
                    await identityClaimRepository.DeleteAsync(identityResource);
                    return await identityClaimRepository.SaveChangesAsync();
                }

                return frameworkResult.Failed<FrameworkResult>(ApiErrorCodes
                    .InvalidIdentityResourceClaimTypeOrResourceId);
            }

            return frameworkResult.Failed<FrameworkResult>(identityResourceModelValidation.ErrorCode);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to remove identity resource claims.");
            throw;
        }
    }

    public virtual async Task<IList<IdentityClaimsModel>> GetIdentityResourceClaimsAsync(Guid identityResourceId)
    {
        if (!identityResourceId.IsValid()) frameworkResult.Throw(ApiErrorCodes.IdentityResourceIdInvalid);

        try
        {
            var identityResource =
                await identityClaimRepository.GetAsync(x => x.IdentityResourceId == identityResourceId);
            if (identityResource.ContainsAny())
            {
                loggerService.WriteTo(Log.Debug, "Entered into get identity resource claims :" + identityResourceId);
                return mapper.Map<IList<IdentityClaims>, IList<IdentityClaimsModel>>(identityResource);
            }

            return frameworkResult.EmptyResult<IList<IdentityClaimsModel>>(ApiErrorCodes.NoRecordsFound);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Failed to get identity resource claims.");
            throw;
        }
    }
}
