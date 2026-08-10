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
using HCL.CS.DomainServices.Wrappers;
using HCL.CS.Service.Implementation.Api.Validators;
using HCL.CS.Service.Interfaces.Interfaces.Api;

namespace HCL.CS.Service.Implementation.Api.Specifications;

internal sealed class UserRoleModelSpecification : BaseDomainModelValidator<UserRoleModel>
{
    internal UserRoleModelSpecification(CrudMode crudMode, UserManagerWrapper<Users> userManager,
        IRoleService roleService)
    {
        Add("CheckValidUserId", new Rule<UserRoleModel>(
            new IsValidIdentifier<UserRoleModel>(model => model.UserId),
            ApiErrorCodes.InvalidUserId));
        Add("CheckValidRoleId", new Rule<UserRoleModel>(
            new IsValidIdentifier<UserRoleModel>(model => model.RoleId),
            ApiErrorCodes.InvalidRoleId));
        Add("CheckCreatedByLength", new Rule<UserRoleModel>(
            new IsValid255CharLength<UserRoleModel>(model => model.CreatedBy),
            ApiErrorCodes.CreatedByTooLong));

        Add("CheckUserExist", new Rule<UserRoleModel>(
            new CheckUserExist(userManager),
            ApiErrorCodes.InvalidUserId));

        Add("CheckRoleExist", new Rule<UserRoleModel>(
            new CheckRoleExist(roleService),
            ApiErrorCodes.InvalidRoleId));

        if (crudMode == CrudMode.Update)
            Add("CheckModifiedByLength", new Rule<UserRoleModel>(
                new IsValid255CharLength<UserRoleModel>(model => model.ModifiedBy),
                ApiErrorCodes.ModifiedByTooLong));
    }

    private class CheckUserExist : ISpecification<UserRoleModel>
    {
        private readonly UserManagerWrapper<Users> userManager;

        internal CheckUserExist(UserManagerWrapper<Users> userManager)
        {
            this.userManager = userManager;
        }

        public bool IsSatisfiedBy(UserRoleModel model)
        {
            var userExist = userManager.FindByIdAsync(model.UserId.ToString()).GetAwaiter().GetResult();
            return userExist != null;
        }
    }

    private class CheckRoleExist : ISpecification<UserRoleModel>
    {
        private readonly IRoleService roleService;

        internal CheckRoleExist(IRoleService roleService)
        {
            this.roleService = roleService;
        }

        public bool IsSatisfiedBy(UserRoleModel model)
        {
            var roleExist = roleService.GetRoleAsync(model.RoleId).GetAwaiter().GetResult();
            return roleExist != null;
        }
    }
}
