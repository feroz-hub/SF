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
