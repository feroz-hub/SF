using DomainValidation.Interfaces.Specification;
using DomainValidation.Validation;
using Zentra.Domain;
using Zentra.Domain.Entities.Api;
using Zentra.Domain.ErrorCodes;
using Zentra.Domain.Models.Api;
using Zentra.DomainServices;
using Zentra.Service.Implementation.Api.Validators;

namespace Zentra.Service.Implementation.Api.Specifications;

public sealed class ApiResourceClaimModelSpecification : BaseDomainModelValidator<ApiResourceClaimsModel>
{
    internal ApiResourceClaimModelSpecification(CrudMode crudMode,
        IRepository<ApiResourceClaims> apiResourceClaimRepository)
    {
        Add("CheckValidApiResourceId", new Rule<ApiResourceClaimsModel>(
            new IsValidIdentifier<ApiResourceClaimsModel>(model => model.ApiResourceId),
            ApiErrorCodes.ApiResourceIdInvalid));

        Add("CheckValidApiResourceClaimType", new Rule<ApiResourceClaimsModel>(
            new IsNotNull<ApiResourceClaimsModel>(model => model.Type),
            ApiErrorCodes.ApiResourceClaimTypeRequired));

        Add("CheckValidApiResourceClaimTypeLength", new Rule<ApiResourceClaimsModel>(
            new IsValid255CharLength<ApiResourceClaimsModel>(model => model.Type),
            ApiErrorCodes.ApiResourceClaimTypeTooLong));

        if (crudMode == CrudMode.Add)
        {
            Add("CheckValidApiResourceClaimCreatedBy", new Rule<ApiResourceClaimsModel>(
                new IsValid255CharLength<ApiResourceClaimsModel>(model => model.CreatedBy),
                ApiErrorCodes.CreatedByTooLong));

            Add("CheckDuplicateApiRecourceClaims", new Rule<ApiResourceClaimsModel>(
                new ApiResourceClaimDuplicateCheck(apiResourceClaimRepository),
                ApiErrorCodes.ApiResourceClaimAlreadyExists));
        }
    }

    private class ApiResourceClaimDuplicateCheck : ISpecification<ApiResourceClaimsModel>
    {
        private readonly IRepository<ApiResourceClaims> apiResourceClaimRepository;

        internal ApiResourceClaimDuplicateCheck(IRepository<ApiResourceClaims> apiResourceClaimRepository)
        {
            this.apiResourceClaimRepository = apiResourceClaimRepository;
        }

        public bool IsSatisfiedBy(ApiResourceClaimsModel model)
        {
            var duplicateExists = apiResourceClaimRepository.DuplicateExistsAsync(apiResourceClaim =>
                    apiResourceClaim.ApiResourceId == model.ApiResourceId &&
                    apiResourceClaim.Type == model.Type)
                .GetAwaiter().GetResult();
            if (duplicateExists) return false;

            return true;
        }
    }
}
