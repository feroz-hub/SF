using DomainValidation.Interfaces.Specification;
using DomainValidation.Validation;
using HCL.CS.Domain;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.Domain.Models.Api;
using HCL.CS.DomainServices;
using HCL.CS.Service.Extension;
using HCL.CS.Service.Implementation.Api.Validators;
using HCL.CS.Service.Implementation.Endpoint.Extensions;

namespace HCL.CS.Service.Implementation.Api.Specifications;

internal sealed class RoleModelSpecification : BaseDomainModelValidator<RoleModel>
{
    internal RoleModelSpecification(CrudMode crudMode, IRepository<ApiScopes> apiScopeRepository)
    {
        // Role
        Add("CheckRoleNameExists", new Rule<RoleModel>(
            new IsNotNull<RoleModel>(model => model.Name),
            ApiErrorCodes.InvalidRoleName));
        Add("CheckRoleNameLength", new Rule<RoleModel>(
            new IsValid255CharLength<RoleModel>(model => model.Name),
            ApiErrorCodes.RoleNameTooLong));
        Add("CheckRoleCreatedByLength", new Rule<RoleModel>(
            new IsValid255CharLength<RoleModel>(model => model.CreatedBy),
            ApiErrorCodes.CreatedByTooLong));

        // Role Claim
        Add("RoleClaimsCheck", new Rule<RoleModel>(
            new RoleClaimsCheck(),
            ApiErrorCodes.InvalidRoleClaimValueOrClaimType));
        Add("CheckValidRoleClaimCreatedBy", new Rule<RoleModel>(
            new IsValid255CharLengths<RoleModel>(model =>
                model.RoleClaims.ContainsAny() ? model.RoleClaims.ConvertAll(x => x.CreatedBy) : null),
            ApiErrorCodes.RoleClaimCreatedByTooLong));

        Add("CheckValidRoleClaim", new Rule<RoleModel>(
            new CheckValidRoleClaim(apiScopeRepository),
            EndpointErrorCodes.InvalidScopeClaims));

        if (crudMode == CrudMode.Update) UpdateRules();
    }

    private void UpdateRules()
    {
        Add("CheckValidRoleId", new Rule<RoleModel>(
            new IsValidIdentifier<RoleModel>(model => model.Id),
            ApiErrorCodes.InvalidRoleId));
        Add("CheckRoleModifiedByLength", new Rule<RoleModel>(
            new IsValid255CharLength<RoleModel>(model => model.ModifiedBy),
            ApiErrorCodes.ModifiedByTooLong));

        Add("CheckValidRoleClaimRoleId", new Rule<RoleModel>(
            new RoleClaimsRoleIdCheck(),
            ApiErrorCodes.InvalidRoleClaimRoleId));
        Add("CheckValidRoleClaimModifiedBy", new Rule<RoleModel>(
            new IsValid255CharLengths<RoleModel>(model =>
                model.RoleClaims.ContainsAny() ? model.RoleClaims.ConvertAll(x => x.ModifiedBy) : null),
            ApiErrorCodes.RoleClaimModifiedByTooLong));
    }

    private class RoleClaimsCheck : ISpecification<RoleModel>
    {
        public bool IsSatisfiedBy(RoleModel model)
        {
            if (!model.RoleClaims.ContainsAny()) return true;
            foreach (var roleClaims in model.RoleClaims)
            {
                if (string.IsNullOrWhiteSpace(roleClaims.ClaimType)) return false;

                if (string.IsNullOrWhiteSpace(roleClaims.ClaimValue)) return false;
            }

            return true;
        }
    }

    private class RoleClaimsRoleIdCheck : ISpecification<RoleModel>
    {
        public bool IsSatisfiedBy(RoleModel model)
        {
            return !model.RoleClaims.ContainsAny() || model.RoleClaims.All(claim => claim.RoleId.IsValid());
        }
    }

    private class CheckValidRoleClaim : ISpecification<RoleModel>
    {
        private readonly IRepository<ApiScopes> apiScopeRepository;

        internal CheckValidRoleClaim(IRepository<ApiScopes> apiScopeRepository)
        {
            this.apiScopeRepository = apiScopeRepository;
        }

        public bool IsSatisfiedBy(RoleModel model)
        {
            if (!model.RoleClaims.ContainsAny()) return true;
            // Only validate ClaimValues that are scope names; capabilities claim values are free-form.
            var scopeValueClaims = model.RoleClaims
                .Where(rc => !string.Equals(rc.ClaimType, OpenIdConstants.ClaimTypes.Capabilities, StringComparison.OrdinalIgnoreCase))
                .Select(rc => rc.ClaimValue)
                .ToList();
            if (scopeValueClaims.Count == 0) return true;
            var commonScopes = apiScopeRepository.GetAllAsync().GetAwaiter().GetResult().Select(x => x.Name.Trim()).ToList();
            if (!scopeValueClaims.Count.Equals(scopeValueClaims.Distinct().Count())) return false;
            return !scopeValueClaims.Except(commonScopes).Any();
        }
    }
}
