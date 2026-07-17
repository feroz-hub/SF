using DomainValidation.Interfaces.Specification;
using DomainValidation.Validation;
using Zentra.Domain;
using Zentra.Domain.Constants;
using Zentra.Domain.Entities.Api;
using Zentra.Domain.ErrorCodes;
using Zentra.Domain.Models.Api;
using Zentra.DomainServices;
using Zentra.Service.Extension;
using Zentra.Service.Implementation.Api.Validators;
using Zentra.Service.Implementation.Endpoint.Extensions;

namespace Zentra.Service.Implementation.Api.Specifications;

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
