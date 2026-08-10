/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using DomainValidation.Interfaces.Specification;
using DomainValidation.Validation;
using HCL.CS.Domain;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.Domain.Models.Api;
using HCL.CS.DomainServices;
using HCL.CS.Service.Implementation.Api.Validators;

namespace HCL.CS.Service.Implementation.Api.Specifications;

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
