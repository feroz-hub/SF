/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using DomainValidation.Validation;
using HCL.CS.Domain;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.Domain.Models.Api;
using HCL.CS.Service.Implementation.Api.Validators;

namespace HCL.CS.Service.Implementation.Api.Specifications;

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
