using DomainValidation.Interfaces.Specification;
using DomainValidation.Validation;
using Zentra.Domain.Constants.Endpoint;
using Zentra.Domain.Entities.Api;
using Zentra.Domain.ErrorCodes;
using Zentra.Domain.Models.Api;
using Zentra.DomainServices;
using Zentra.Service.Implementation.Api.Validators;

namespace Zentra.Service.Implementation.Api.Specifications;

internal sealed class RoleClaimModelSpecification : BaseDomainModelValidator<RoleClaimModel>
{
    internal RoleClaimModelSpecification(IRepository<ApiScopes> apiScopeRepository)
    {
        Add("CheckRoleClaimModel", new Rule<RoleClaimModel>(
            new IsNotNull<RoleClaimModel>(model => model),
            ApiErrorCodes.InvalidRoleClaim));
        Add("CheckRoleId", new Rule<RoleClaimModel>(
            new IsValidIdentifier<RoleClaimModel>(model => model.RoleId),
            ApiErrorCodes.InvalidRoleId));
        Add("CheckValidClaimType", new Rule<RoleClaimModel>(
            new IsNotNull<RoleClaimModel>(model => model.ClaimType),
            ApiErrorCodes.RoleClaimTypeRequired));
        Add("CheckValidClaimValue", new Rule<RoleClaimModel>(
            new IsNotNull<RoleClaimModel>(model => model.ClaimValue),
            ApiErrorCodes.RoleClaimValueRequired));
        Add("CheckRoleClaimCreatedByLength", new Rule<RoleClaimModel>(
            new IsValid255CharLength<RoleClaimModel>(model => model.CreatedBy),
            ApiErrorCodes.CreatedByTooLong));

        Add("CheckValidRoleClaim", new Rule<RoleClaimModel>(
            new CheckValidRoleClaim(apiScopeRepository),
            EndpointErrorCodes.InvalidScopeClaims));
    }

    internal class RoleClaimsTypeCheck : ISpecification<RoleClaimModel>
    {
        public bool IsSatisfiedBy(RoleClaimModel model)
        {
            if (string.IsNullOrWhiteSpace(model.ClaimType)) return false;

            return true;
        }
    }

    internal class RoleClaimsValueCheck : ISpecification<RoleClaimModel>
    {
        public bool IsSatisfiedBy(RoleClaimModel model)
        {
            if (string.IsNullOrWhiteSpace(model.ClaimValue)) return false;

            return true;
        }
    }

    private class CheckValidRoleClaim : ISpecification<RoleClaimModel>
    {
        private readonly IRepository<ApiScopes> apiScopeRepository;

        internal CheckValidRoleClaim(IRepository<ApiScopes> apiScopeRepository)
        {
            this.apiScopeRepository = apiScopeRepository;
        }

        public bool IsSatisfiedBy(RoleClaimModel? roleClaimModel)
        {
            if (roleClaimModel == null) return true;
            // Capabilities claim values are free-form (e.g. "health:read"); they are not ApiScope names.
            if (string.Equals(roleClaimModel.ClaimType, OpenIdConstants.ClaimTypes.Capabilities, StringComparison.OrdinalIgnoreCase))
                return true;
            var commonScopes = apiScopeRepository.GetAllAsync().GetAwaiter().GetResult().Select(x => x.Name.Trim());

            List<string> allowedScopes =
            [
                roleClaimModel.ClaimValue
            ];
            if (!allowedScopes.Count.Equals(allowedScopes.Distinct().Count())) return false;
            return !allowedScopes.Except(commonScopes.ToList()).Any();
        }
    }
}
