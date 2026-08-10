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

public sealed class IdentityClaimModelSpecification : BaseDomainModelValidator<IdentityClaimsModel>
{
    internal IdentityClaimModelSpecification(CrudMode crudMode, IRepository<IdentityClaims> identityClaimRepository)
    {
        Add("CheckValidIdentityResourceId", new Rule<IdentityClaimsModel>(
            new IsValidIdentifier<IdentityClaimsModel>(model => model.IdentityResourceId),
            ApiErrorCodes.IdentityResourceIdInvalid));

        Add("CheckValidIdentityResourceClaimType", new Rule<IdentityClaimsModel>(
            new IsNotNull<IdentityClaimsModel>(model => model.Type),
            ApiErrorCodes.IdentityResourceClaimTypeRequired));

        Add("CheckValidIdentityResourceClaimTypeLength", new Rule<IdentityClaimsModel>(
            new IsValid255CharLength<IdentityClaimsModel>(model => model.Type),
            ApiErrorCodes.IdentityResourceClaimTypeTooLong));

        if (crudMode == CrudMode.Add)
        {
            Add("CheckValidIdentityResourceClaimCreatedBy", new Rule<IdentityClaimsModel>(
                new IsValid255CharLength<IdentityClaimsModel>(model => model.CreatedBy),
                ApiErrorCodes.CreatedByTooLong));

            Add("CheckDuplicateIdentityRecourceClaims", new Rule<IdentityClaimsModel>(
                new IdentityResourceClaimDuplicateCheck(identityClaimRepository),
                ApiErrorCodes.IdentityResourceClaimAlreadyExists));
        }
    }

    private class IdentityResourceClaimDuplicateCheck : ISpecification<IdentityClaimsModel>
    {
        private readonly IRepository<IdentityClaims> identityResourceClaimRepository;

        internal IdentityResourceClaimDuplicateCheck(IRepository<IdentityClaims> identityResourceClaimRepository)
        {
            this.identityResourceClaimRepository = identityResourceClaimRepository;
        }

        public bool IsSatisfiedBy(IdentityClaimsModel model)
        {
            var duplicateExists = identityResourceClaimRepository.DuplicateExistsAsync(apiResourceClaim =>
                    apiResourceClaim.IdentityResourceId == model.IdentityResourceId &&
                    apiResourceClaim.Type == model.Type)
                .GetAwaiter().GetResult();
            if (duplicateExists) return false;

            return true;
        }
    }
}
