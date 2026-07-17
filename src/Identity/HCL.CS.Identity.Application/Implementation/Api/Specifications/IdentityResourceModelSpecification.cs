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

internal sealed class IdentityResourceModelSpecification : BaseDomainModelValidator<IdentityResourcesModel>
{
    internal IdentityResourceModelSpecification(CrudMode crudMode,
        IRepository<IdentityResources> identityResourceRepository)
    {
        Add("CheckIdentityResourceName", new Rule<IdentityResourcesModel>(
            new IsNotNull<IdentityResourcesModel>(model => model.Name),
            ApiErrorCodes.IdentityResourceNameRequired));

        Add("CheckIdentityResourceNameLength", new Rule<IdentityResourcesModel>(
            new IsValid255CharLength<IdentityResourcesModel>(model => model.Name),
            ApiErrorCodes.IdentityResourceNameTooLong));

        Add("CheckIdentityResourceDisplayNameLength", new Rule<IdentityResourcesModel>(
            new IsValid255CharLength<IdentityResourcesModel>(model => model.DisplayName),
            ApiErrorCodes.IdentityDisplayNameTooLong));

        Add("CheckIdentityResourceCreatedByLength", new Rule<IdentityResourcesModel>(
            new IsValid255CharLength<IdentityResourcesModel>(model => model.CreatedBy),
            ApiErrorCodes.CreatedByTooLong));

        switch (crudMode)
        {
            case CrudMode.Add:
                AddRules(identityResourceRepository);
                break;
            case CrudMode.Update:
                UpdateRules();
                break;
        }
    }

    private void AddRules(IRepository<IdentityResources> identityResourceRepository)
    {
        // Identity resource.
        Add("CheckValidIdentityResourceClaimType", new Rule<IdentityResourcesModel>(
            new IdentityClaimsTypeCheck(),
            ApiErrorCodes.IdentityResourceClaimTypeRequired));
        Add("CheckValidIdentityResourceClaimTypeLength", new Rule<IdentityResourcesModel>(
            new IsValid255CharLengths<IdentityResourcesModel>(model =>
                model.IdentityClaims.ContainsAny() ? model.IdentityClaims.ConvertAll(x => x.Type) : null),
            ApiErrorCodes.IdentityResourceClaimTypeTooLong));
        Add("CheckValidIdentityResourceClaimCreatedBy", new Rule<IdentityResourcesModel>(
            new IsValid255CharLengths<IdentityResourcesModel>(model =>
                model.IdentityClaims.ContainsAny() ? model.IdentityClaims.ConvertAll(x => x.CreatedBy) : null),
            ApiErrorCodes.IdentityResourceClaimCreatedByTooLong));
        Add("CheckDuplicateIdentityRecourceEntry", new Rule<IdentityResourcesModel>(
            new IdentityResourceDuplicateCheck(identityResourceRepository),
            ApiErrorCodes.IdentityResourceAlreadyExists));
    }

    private void UpdateRules()
    {
        // Identity resource.
        Add("CheckIdentityResourceId", new Rule<IdentityResourcesModel>(
            new IsValidIdentifier<IdentityResourcesModel>(model => model.Id),
            ApiErrorCodes.IdentityResourceIdInvalid));
        Add("CheckIdentityResourceModifiedByLength", new Rule<IdentityResourcesModel>(
            new IsValid255CharLength<IdentityResourcesModel>(model => model.ModifiedBy),
            ApiErrorCodes.ModifiedByTooLong));

        // Identity resource claim.
        Add("CheckValidIdentityResourceClaim", new Rule<IdentityResourcesModel>(
            new IdentityResourceClaimsCheck(),
            ApiErrorCodes.InvalidIdentityResourceClaimTypeOrResourceId));
        Add("CheckValidIdentityResourceClaimTypeLength", new Rule<IdentityResourcesModel>(
            new IsValid255CharLengths<IdentityResourcesModel>(model =>
                model.IdentityClaims.ContainsAny() ? model.IdentityClaims.ConvertAll(x => x.Type) : null),
            ApiErrorCodes.IdentityResourceClaimTypeTooLong));
        Add("CheckValidIdentityResourceClaimCreatedModifiedByLength", new Rule<IdentityResourcesModel>(
            new IdentityResourceClaimsCreatedModifiedByCheck(),
            ApiErrorCodes.IdentityResourceClaimCreatedbyOrModifiedByTooLong));
    }

    internal class IdentityClaimsTypeCheck : ISpecification<IdentityResourcesModel>
    {
        public bool IsSatisfiedBy(IdentityResourcesModel model)
        {
            if (model.IdentityClaims.ContainsAny())
                foreach (var identityClaim in model.IdentityClaims)
                    if (string.IsNullOrWhiteSpace(identityClaim.Type))
                        return false;

            return true;
        }
    }

    internal class IdentityResourceDuplicateCheck : ISpecification<IdentityResourcesModel>
    {
        private readonly IRepository<IdentityResources> identityResourceRepository;

        internal IdentityResourceDuplicateCheck(IRepository<IdentityResources> identityResourceRepository)
        {
            this.identityResourceRepository = identityResourceRepository;
        }

        public bool IsSatisfiedBy(IdentityResourcesModel model)
        {
            var duplicateExists = identityResourceRepository
                .DuplicateExistsAsync(idResource => idResource.Name == model.Name)
                .GetAwaiter().GetResult();
            if (duplicateExists) return false;

            return true;
        }
    }

    internal class IdentityResourceClaimsCheck : ISpecification<IdentityResourcesModel>
    {
        public bool IsSatisfiedBy(IdentityResourcesModel model)
        {
            if (model.IdentityClaims.ContainsAny())
                foreach (var identityClaims in model.IdentityClaims)
                    if (!identityClaims.IdentityResourceId.IsValid() || string.IsNullOrWhiteSpace(identityClaims.Type))
                        return false;

            return true;
        }
    }

    internal class IdentityResourceClaimsCreatedModifiedByCheck : ISpecification<IdentityResourcesModel>
    {
        public bool IsSatisfiedBy(IdentityResourcesModel model)
        {
            if (model.IdentityClaims.ContainsAny())
                foreach (var apiClaim in model.IdentityClaims)
                {
                    if (!string.IsNullOrWhiteSpace(apiClaim.CreatedBy) &&
                        apiClaim.CreatedBy.Length > Constants.ColumnLength255) return false;

                    if (!string.IsNullOrWhiteSpace(apiClaim.ModifiedBy) &&
                        apiClaim.ModifiedBy.Length > Constants.ColumnLength255) return false;
                }

            return true;
        }
    }
}
