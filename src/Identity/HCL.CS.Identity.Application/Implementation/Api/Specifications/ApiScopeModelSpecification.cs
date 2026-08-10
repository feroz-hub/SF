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
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.Domain.Models.Api;
using HCL.CS.DomainServices;
using HCL.CS.Service.Extension;
using HCL.CS.Service.Implementation.Api.Validators;
using HCL.CS.Service.Implementation.Endpoint.Extensions;

namespace HCL.CS.Service.Implementation.Api.Specifications;

internal sealed class ApiScopeModelSpecification : BaseDomainModelValidator<ApiScopesModel>
{
    internal ApiScopeModelSpecification(CrudMode crudMode, IRepository<ApiScopes> apiScopeRepository)
    {
        Add("CheckValidApiResourceId", new Rule<ApiScopesModel>(
            new IsValidIdentifier<ApiScopesModel>(model => model.ApiResourceId),
            ApiErrorCodes.ApiResourceIdInvalid));

        Add("CheckValidApiScopeName", new Rule<ApiScopesModel>(
            new IsNotNull<ApiScopesModel>(model => model.Name),
            ApiErrorCodes.ApiScopeNameRequired));

        Add("CheckValidApiScopeNameLength", new Rule<ApiScopesModel>(
            new IsValid255CharLength<ApiScopesModel>(model => model.Name),
            ApiErrorCodes.ApiScopeNameTooLong));

        Add("CheckValidApiScopeDisplayNameLength", new Rule<ApiScopesModel>(
            new IsValid255CharLength<ApiScopesModel>(model => model.DisplayName),
            ApiErrorCodes.ApiScopeDisplayNameTooLong));

        Add("CheckValidApiScopeCreatedByLength", new Rule<ApiScopesModel>(
            new IsValid255CharLength<ApiScopesModel>(model => model.CreatedBy),
            ApiErrorCodes.CreatedByTooLong));

        switch (crudMode)
        {
            case CrudMode.Add:
                AddRules(apiScopeRepository);
                break;
            case CrudMode.Update:
                UpdateRules(apiScopeRepository);
                break;
        }
    }

    private void AddRules(IRepository<ApiScopes> apiScopeRepository)
    {
        // Api scope claim
        Add("CheckValidApiScopeClaimType", new Rule<ApiScopesModel>(
            new ApiScopeClaimsTypeCheck(),
            ApiErrorCodes.ApiScopeClaimTypeRequired));
        Add("CheckValidApiScopeClaimTypeAndCreatedbyLength", new Rule<ApiScopesModel>(
            new ApiScopeClaimsTypeLengthCheck(),
            ApiErrorCodes.ApiScopeClaimTypeOrCreatedbyTooLong));
        Add("CheckDuplicateApiRecourceEntry", new Rule<ApiScopesModel>(
            new ApiScopeDuplicateCheck(apiScopeRepository),
            ApiErrorCodes.ApiScopeAlreadyExists));
    }

    private void UpdateRules(IRepository<ApiScopes> apiScopeRepository)
    {
        // Api scope
        Add("CheckValidApiScopeId", new Rule<ApiScopesModel>(
            new IsValidIdentifier<ApiScopesModel>(model => model.Id),
            ApiErrorCodes.ApiScopeIdInvalid));
        Add("CheckValidApiScopeModifiedByLength", new Rule<ApiScopesModel>(
            new IsValid255CharLength<ApiScopesModel>(model => model.ModifiedBy),
            ApiErrorCodes.ModifiedByTooLong));

        // Api scope claim
        Add("CheckValidApiScopeClaim", new Rule<ApiScopesModel>(
            new ApiScopeClaimsCheck(),
            ApiErrorCodes.InvalidApiScopeClaimTypeOrScopeId));
        Add("CheckValidApiScopeClaimTypeLength", new Rule<ApiScopesModel>(
            new ApiScopeClaimsTypeLengthCheck(),
            ApiErrorCodes.ApiScopeClaimTypeOrCreatedbyTooLong));
        Add("CheckValidApiScopeClaimModifiedByLength", new Rule<ApiScopesModel>(
            new IsValid255CharLengths<ApiScopesModel>(model =>
                model.ApiScopeClaims.ContainsAny() ? model.ApiScopeClaims.ConvertAll(x => x.ModifiedBy) : null),
            ApiErrorCodes.ApiScopeClaimModifiedbyTooLong));
        Add("CheckActiveApiScopeEntry", new Rule<ApiScopesModel>(
            new ApiScopeActiveCheck(apiScopeRepository),
            ApiErrorCodes.ApiScopeNameInvalid));
    }

    private class ApiScopeClaimsTypeCheck : ISpecification<ApiScopesModel>
    {
        public bool IsSatisfiedBy(ApiScopesModel model)
        {
            if (model.ApiScopeClaims.ContainsAny())
                foreach (var apiScopeClaims in model.ApiScopeClaims)
                    if (string.IsNullOrWhiteSpace(apiScopeClaims.Type))
                        return false;

            return true;
        }
    }

    private class ApiScopeClaimsTypeLengthCheck : ISpecification<ApiScopesModel>
    {
        public bool IsSatisfiedBy(ApiScopesModel model)
        {
            if (model.ApiScopeClaims.ContainsAny())
                foreach (var apiScopeClaims in model.ApiScopeClaims)
                {
                    if (!string.IsNullOrWhiteSpace(apiScopeClaims.Type) &&
                        apiScopeClaims.Type.Length > Constants.ColumnLength255) return false;

                    if (!string.IsNullOrWhiteSpace(apiScopeClaims.CreatedBy) &&
                        apiScopeClaims.CreatedBy.Length > Constants.ColumnLength255) return false;
                }

            return true;
        }
    }

    private class ApiScopeClaimsCheck : ISpecification<ApiScopesModel>
    {
        public bool IsSatisfiedBy(ApiScopesModel model)
        {
            if (model.ApiScopeClaims.ContainsAny())
                foreach (var apiScopeClaims in model.ApiScopeClaims)
                {
                    if (!apiScopeClaims.ApiScopeId.IsValid()) return false;

                    if (string.IsNullOrWhiteSpace(apiScopeClaims.Type)) return false;
                }

            return true;
        }
    }

    private class ApiScopeDuplicateCheck : ISpecification<ApiScopesModel>
    {
        private readonly IRepository<ApiScopes> apiScopeRepository;

        internal ApiScopeDuplicateCheck(IRepository<ApiScopes> apiScopeRepository)
        {
            this.apiScopeRepository = apiScopeRepository;
        }

        public bool IsSatisfiedBy(ApiScopesModel model)
        {
            var duplicateExists = apiScopeRepository.DuplicateExistsAsync(apiScope => apiScope.Name == model.Name)
                .GetAwaiter().GetResult();
            if (duplicateExists) return false;

            return true;
        }
    }

    private class ApiScopeActiveCheck : ISpecification<ApiScopesModel>
    {
        private readonly IRepository<ApiScopes> apiScopeRepository;

        internal ApiScopeActiveCheck(IRepository<ApiScopes> apiScopeRepository)
        {
            this.apiScopeRepository = apiScopeRepository;
        }

        public bool IsSatisfiedBy(ApiScopesModel model)
        {
            var isResourceExists = apiScopeRepository.ActiveRecordExistsAsync(apiscope => apiscope.Id == model.Id)
                .GetAwaiter().GetResult();
            if (isResourceExists) return true;

            return false;
        }
    }
}
