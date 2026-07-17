using DomainValidation.Interfaces.Specification;
using DomainValidation.Validation;
using HCL.CS.Domain;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.Domain.Models.Api;
using HCL.CS.DomainServices;
using HCL.CS.Service.Implementation.Api.Validators;

namespace HCL.CS.Service.Implementation.Api.Specifications;

public sealed class ApiScopeClaimModelSpecification : BaseDomainModelValidator<ApiScopeClaimsModel>
{
    internal ApiScopeClaimModelSpecification(CrudMode crudMode, IRepository<ApiScopeClaims> apiScopeClaimsRepository)
    {
        Add("CheckValidApiScopeId", new Rule<ApiScopeClaimsModel>(
            new IsValidIdentifier<ApiScopeClaimsModel>(model => model.ApiScopeId),
            ApiErrorCodes.ApiScopeIdInvalid));

        Add("CheckValidApiScopeClaimType", new Rule<ApiScopeClaimsModel>(
            new IsNotNull<ApiScopeClaimsModel>(model => model.Type),
            ApiErrorCodes.ApiScopeClaimTypeRequired));

        Add("CheckValidApiScopeClaimTypeLength", new Rule<ApiScopeClaimsModel>(
            new IsValid255CharLength<ApiScopeClaimsModel>(model => model.Type),
            ApiErrorCodes.ApiScopeClaimTypeTooLong));

        if (crudMode == CrudMode.Add)
        {
            Add("CheckValidApiScopeClaimCreatedBy", new Rule<ApiScopeClaimsModel>(
                new IsValid255CharLength<ApiScopeClaimsModel>(model => model.CreatedBy),
                ApiErrorCodes.CreatedByTooLong));

            Add("CheckDuplicateApiScopeClaims", new Rule<ApiScopeClaimsModel>(
                new ApiScopeClaimDuplicateCheck(apiScopeClaimsRepository),
                ApiErrorCodes.ApiScopeClaimsAlreadyExists));
        }
    }

    private class ApiScopeClaimDuplicateCheck : ISpecification<ApiScopeClaimsModel>
    {
        private readonly IRepository<ApiScopeClaims> apiScopeClaimsRepository;

        internal ApiScopeClaimDuplicateCheck(IRepository<ApiScopeClaims> apiScopeClaimsRepository)
        {
            this.apiScopeClaimsRepository = apiScopeClaimsRepository;
        }

        public bool IsSatisfiedBy(ApiScopeClaimsModel model)
        {
            var duplicateExists = apiScopeClaimsRepository.DuplicateExistsAsync(apiScopeClaim =>
                    apiScopeClaim.ApiScopeId == model.ApiScopeId &&
                    apiScopeClaim.Type == model.Type)
                .GetAwaiter().GetResult();
            if (duplicateExists) return false;

            return true;
        }
    }
}
