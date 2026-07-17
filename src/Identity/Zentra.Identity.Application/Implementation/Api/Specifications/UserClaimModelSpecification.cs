using DomainValidation.Validation;
using Zentra.Domain;
using Zentra.Domain.ErrorCodes;
using Zentra.Domain.Models.Api;
using Zentra.Service.Implementation.Api.Validators;

namespace Zentra.Service.Implementation.Api.Specifications;

internal sealed class UserClaimModelSpecification : BaseDomainModelValidator<UserClaimModel>
{
    internal UserClaimModelSpecification(CrudMode crudMode)
    {
        Add("CheckUserClaimModel", new Rule<UserClaimModel>(
            new IsNotNull<UserClaimModel>(model => model),
            ApiErrorCodes.InvalidUserClaims));
        Add("CheckUserClaimUserId", new Rule<UserClaimModel>(
            new IsNotNull<UserClaimModel>(model => model.UserId),
            ApiErrorCodes.InvalidUserId));
        Add("CheckValidClaimType", new Rule<UserClaimModel>(
            new IsNotNull<UserClaimModel>(model => model.ClaimType),
            ApiErrorCodes.UserClaimTypeRequired));
        Add("CheckValidClaimValue", new Rule<UserClaimModel>(
            new IsNotNull<UserClaimModel>(model => model.ClaimValue),
            ApiErrorCodes.UserClaimValueRequired));
        Add("CheckCreatedByLength", new Rule<UserClaimModel>(
            new IsValid255CharLength<UserClaimModel>(model => model.CreatedBy),
            ApiErrorCodes.CreatedByTooLong));

        if (crudMode == CrudMode.Update)
            Add("CheckModifiedByLength", new Rule<UserClaimModel>(
                new IsValid255CharLength<UserClaimModel>(model => model.ModifiedBy),
                ApiErrorCodes.ModifiedByTooLong));
    }
}
