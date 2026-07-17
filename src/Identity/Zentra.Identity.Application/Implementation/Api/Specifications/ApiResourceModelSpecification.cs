using DomainValidation.Interfaces.Specification;
using DomainValidation.Validation;
using Zentra.Domain;
using Zentra.Domain.Constants;
using Zentra.Domain.ErrorCodes;
using Zentra.Domain.Models.Api;
using Zentra.DomainServices.Repository.Api;
using Zentra.Service.Extension;
using Zentra.Service.Implementation.Api.Validators;
using Zentra.Service.Implementation.Endpoint.Extensions;

namespace Zentra.Service.Implementation.Api.Specifications;

internal sealed class ApiResourceModelSpecification : BaseDomainModelValidator<ApiResourcesModel>
{
    internal ApiResourceModelSpecification(CrudMode crudMode, IApiResourceRepository apiResourceRepository)
    {
        Add("CheckValidApiResourceName", new Rule<ApiResourcesModel>(
            new IsNotNull<ApiResourcesModel>(model => model.Name),
            ApiErrorCodes.ApiResourceNameRequired));

        Add("CheckApiResourceNameLength", new Rule<ApiResourcesModel>(
            new IsValid255CharLength<ApiResourcesModel>(model => model.Name),
            ApiErrorCodes.ApiResourceNameTooLong));

        Add("CheckApiResourceDisplayNameLength", new Rule<ApiResourcesModel>(
            new IsValid255CharLength<ApiResourcesModel>(model => model.DisplayName),
            ApiErrorCodes.ApiResourceDisplayNameTooLong));

        Add("CheckApiResourceCreatedByLength", new Rule<ApiResourcesModel>(
            new IsValid255CharLength<ApiResourcesModel>(model => model.CreatedBy),
            ApiErrorCodes.CreatedByTooLong));

        switch (crudMode)
        {
            case CrudMode.Add:
                AddRules(apiResourceRepository);
                break;
            case CrudMode.Update:
                UpdateRules();
                break;
        }
    }

    private void AddRules(IApiResourceRepository apiResourceRepository)
    {
        // Api resource claim
        Add("CheckValidApiResourceClaimType", new Rule<ApiResourcesModel>(
            new ApiResourceClaimsTypeCheck(),
            ApiErrorCodes.ApiResourceClaimTypeRequired));
        Add("CheckValidApiResourceClaimTypeLength", new Rule<ApiResourcesModel>(
            new IsValid255CharLengths<ApiResourcesModel>(model =>
                model.ApiResourceClaims.ContainsAny() ? model.ApiResourceClaims.ConvertAll(x => x.Type) : null),
            ApiErrorCodes.ApiResourceClaimTypeTooLong));
        Add("CheckValidApiResourceClaimCreatedBy", new Rule<ApiResourcesModel>(
            new IsValid255CharLengths<ApiResourcesModel>(model =>
                model.ApiResourceClaims.ContainsAny() ? model.ApiResourceClaims.ConvertAll(x => x.CreatedBy) : null),
            ApiErrorCodes.ApiResourceClaimCreatedByTooLong));

        // Api scope
        Add("CheckValidApiResourceScopeName", new Rule<ApiResourcesModel>(
            new ApiScopesNameCheck(),
            ApiErrorCodes.ApiScopeNameRequired));
        Add("CheckValidApiResourceScopeLength", new Rule<ApiResourcesModel>(
            new ApiScopesLengthCheck(),
            ApiErrorCodes.ApiScopeNameOrDisplayNameOrCreatedbyTooLong));

        // Api scope claims
        Add("CheckValidApiResourceScopeClaimType", new Rule<ApiResourcesModel>(
            new ApiScopeClaimsTypeCheck(),
            ApiErrorCodes.ApiScopeClaimTypeRequired));
        Add("CheckValidApiResourceScopeClaimTypeAndCreatedbyLength", new Rule<ApiResourcesModel>(
            new ApiScopeClaimsTypeLengthCheck(),
            ApiErrorCodes.ApiScopeClaimTypeOrCreatedbyTooLong));
        Add("CheckDuplicateApiRecourceEntry", new Rule<ApiResourcesModel>(
            new ApiResourceDuplicateCheck(apiResourceRepository),
            ApiErrorCodes.ApiResourceAlreadyExists));
    }

    private void UpdateRules()
    {
        // Api resource
        Add("CheckValidApiResourceId", new Rule<ApiResourcesModel>(
            new IsValidIdentifier<ApiResourcesModel>(model => model.Id),
            ApiErrorCodes.ApiResourceIdInvalid));
        Add("CheckApiResourceModifiedByLength", new Rule<ApiResourcesModel>(
            new IsValid255CharLength<ApiResourcesModel>(model => model.ModifiedBy),
            ApiErrorCodes.ModifiedByTooLong));

        // Api resource claim
        Add("CheckValidApiResourceClaim", new Rule<ApiResourcesModel>(
            new ApiResourceClaimsCheck(),
            ApiErrorCodes.InvalidApiResourceClaimTypeOrResourceId));
        Add("CheckValidApiResourceClaimTypeLength", new Rule<ApiResourcesModel>(
            new IsValid255CharLengths<ApiResourcesModel>(model =>
                model.ApiResourceClaims.ContainsAny() ? model.ApiResourceClaims.ConvertAll(x => x.Type) : null),
            ApiErrorCodes.ApiResourceClaimTypeTooLong));
        Add("CheckValidApiResourceClaimCreatedModifiedByLength", new Rule<ApiResourcesModel>(
            new ApiResourceClaimsCreatedModifiedByCheck(),
            ApiErrorCodes.ApiResourceClaimCreatedbyOrModifiedByTooLong));

        // Api scope
        Add("CheckValidApiResourceScope", new Rule<ApiResourcesModel>(
            new ApiScopesCheck(),
            ApiErrorCodes.InvalidApiScopeNameOrResourceId));
        Add("CheckValidApiResourceScopeLength", new Rule<ApiResourcesModel>(
            new ApiScopesLengthCheck(),
            ApiErrorCodes.ApiScopeNameOrDisplayNameOrCreatedbyTooLong));
        Add("CheckValidApiResourceScopeModifiedByLength", new Rule<ApiResourcesModel>(
            new IsValid255CharLengths<ApiResourcesModel>(model =>
                model.ApiScopes.ContainsAny() ? model.ApiScopes.ConvertAll(x => x.ModifiedBy) : null),
            ApiErrorCodes.ApiScopeModifiedbyTooLong));

        // Api scope claims
        Add("CheckValidApiResourceScopeClaim", new Rule<ApiResourcesModel>(
            new ApiScopeClaimsCheck(),
            ApiErrorCodes.InvalidApiScopeClaimTypeOrScopeId));
        Add("CheckValidApiResourceScopeClaimTypeLength", new Rule<ApiResourcesModel>(
            new ApiScopeClaimsTypeLengthCheck(),
            ApiErrorCodes.ApiScopeClaimTypeOrCreatedbyTooLong));
        Add("CheckValidApiResourceScopeClaimModifiedByLength", new Rule<ApiResourcesModel>(
            new ApiScopeClaimsModifiedByLengthCheck(),
            ApiErrorCodes.ApiScopeClaimModifiedbyTooLong));
    }

    internal class ApiResourceClaimsTypeCheck : ISpecification<ApiResourcesModel>
    {
        public bool IsSatisfiedBy(ApiResourcesModel model)
        {
            if (model.ApiResourceClaims.ContainsAny())
                foreach (var apiClaim in model.ApiResourceClaims)
                    if (string.IsNullOrWhiteSpace(apiClaim.Type))
                        return false;

            return true;
        }
    }

    internal class ApiResourceClaimsCreatedModifiedByCheck : ISpecification<ApiResourcesModel>
    {
        public bool IsSatisfiedBy(ApiResourcesModel model)
        {
            if (model.ApiResourceClaims.ContainsAny())
                foreach (var apiClaim in model.ApiResourceClaims)
                {
                    if (!string.IsNullOrWhiteSpace(apiClaim.CreatedBy) &&
                        apiClaim.CreatedBy.Length > Constants.ColumnLength255) return false;

                    if (!string.IsNullOrWhiteSpace(apiClaim.ModifiedBy) &&
                        apiClaim.ModifiedBy.Length > Constants.ColumnLength255) return false;
                }

            return true;
        }
    }

    internal class ApiScopesNameCheck : ISpecification<ApiResourcesModel>
    {
        public bool IsSatisfiedBy(ApiResourcesModel model)
        {
            if (model.ApiScopes.ContainsAny())
                foreach (var apiScope in model.ApiScopes)
                    if (string.IsNullOrWhiteSpace(apiScope.Name))
                        return false;

            return true;
        }
    }

    internal class ApiScopesLengthCheck : ISpecification<ApiResourcesModel>
    {
        public bool IsSatisfiedBy(ApiResourcesModel model)
        {
            if (model.ApiScopes.ContainsAny())
                foreach (var apiScope in model.ApiScopes)
                {
                    if (!string.IsNullOrWhiteSpace(apiScope.Name) && apiScope.Name.Length > Constants.ColumnLength255)
                        return false;

                    if (!string.IsNullOrWhiteSpace(apiScope.DisplayName) &&
                        apiScope.DisplayName.Length > Constants.ColumnLength255) return false;

                    if (!string.IsNullOrWhiteSpace(apiScope.CreatedBy) &&
                        apiScope.CreatedBy.Length > Constants.ColumnLength255) return false;
                }

            return true;
        }
    }

    internal class ApiScopeClaimsTypeCheck : ISpecification<ApiResourcesModel>
    {
        public bool IsSatisfiedBy(ApiResourcesModel model)
        {
            if (model.ApiScopes.ContainsAny())
                foreach (var apiScope in model.ApiScopes)
                    if (apiScope.ApiScopeClaims.ContainsAny())
                        foreach (var apiScopeClaims in apiScope.ApiScopeClaims)
                            if (string.IsNullOrWhiteSpace(apiScopeClaims.Type))
                                return false;

            return true;
        }
    }

    internal class ApiScopeClaimsTypeLengthCheck : ISpecification<ApiResourcesModel>
    {
        public bool IsSatisfiedBy(ApiResourcesModel model)
        {
            if (model.ApiScopes.ContainsAny())
                foreach (var apiScope in model.ApiScopes)
                    if (apiScope.ApiScopeClaims.ContainsAny())
                        foreach (var apiScopeClaims in apiScope.ApiScopeClaims)
                        {
                            if (!string.IsNullOrWhiteSpace(apiScopeClaims.Type) &&
                                apiScopeClaims.Type.Length > Constants.ColumnLength255) return false;

                            if (!string.IsNullOrWhiteSpace(apiScopeClaims.CreatedBy) &&
                                apiScopeClaims.CreatedBy.Length > Constants.ColumnLength255) return false;
                        }

            return true;
        }
    }

    internal class ApiScopeClaimsModifiedByLengthCheck : ISpecification<ApiResourcesModel>
    {
        public bool IsSatisfiedBy(ApiResourcesModel model)
        {
            if (model.ApiScopes.ContainsAny())
                foreach (var apiScope in model.ApiScopes)
                    if (apiScope.ApiScopeClaims.ContainsAny())
                        foreach (var apiScopeClaims in apiScope.ApiScopeClaims)
                            if (!string.IsNullOrWhiteSpace(apiScopeClaims.ModifiedBy) &&
                                apiScopeClaims.ModifiedBy.Length > Constants.ColumnLength255)
                                return false;

            return true;
        }
    }

    internal class ApiResourceClaimsCheck : ISpecification<ApiResourcesModel>
    {
        public bool IsSatisfiedBy(ApiResourcesModel model)
        {
            if (model.ApiResourceClaims.ContainsAny())
                foreach (var apiClaim in model.ApiResourceClaims)
                {
                    if (!apiClaim.ApiResourceId.IsValid()) return false;

                    if (string.IsNullOrWhiteSpace(apiClaim.Type)) return false;
                }

            return true;
        }
    }

    internal class ApiScopesCheck : ISpecification<ApiResourcesModel>
    {
        public bool IsSatisfiedBy(ApiResourcesModel model)
        {
            if (model.ApiScopes.ContainsAny())
                foreach (var apiScope in model.ApiScopes)
                {
                    if (!apiScope.ApiResourceId.IsValid()) return false;

                    if (string.IsNullOrWhiteSpace(apiScope.Name)) return false;
                }

            return true;
        }
    }

    internal class ApiScopeClaimsCheck : ISpecification<ApiResourcesModel>
    {
        public bool IsSatisfiedBy(ApiResourcesModel model)
        {
            if (model.ApiScopes.ContainsAny())
                foreach (var apiScope in model.ApiScopes)
                    if (apiScope.ApiScopeClaims.ContainsAny())
                        foreach (var apiScopeClaims in apiScope.ApiScopeClaims)
                        {
                            if (!apiScopeClaims.ApiScopeId.IsValid()) return false;

                            if (string.IsNullOrWhiteSpace(apiScopeClaims.Type)) return false;
                        }

            return true;
        }
    }

    internal class ApiResourceDuplicateCheck : ISpecification<ApiResourcesModel>
    {
        private readonly IApiResourceRepository apiResourceRepository;

        internal ApiResourceDuplicateCheck(IApiResourceRepository apiResourceRepository)
        {
            this.apiResourceRepository = apiResourceRepository;
        }

        public bool IsSatisfiedBy(ApiResourcesModel model)
        {
            var duplicateExists = apiResourceRepository
                .DuplicateExistsAsync(apiResource => apiResource.Name == model.Name).GetAwaiter().GetResult();
            if (duplicateExists) return false;

            return true;
        }
    }
}
