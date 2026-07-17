using DomainValidation.Interfaces.Specification;
using DomainValidation.Validation;
using Zentra.Domain;
using Zentra.Domain.Entities.Api;
using Zentra.Domain.ErrorCodes;
using Zentra.Domain.Models.Api;
using Zentra.DomainServices.Wrappers;
using Zentra.Service.Implementation.Api.Validators;
using Zentra.Service.Interfaces.Interfaces.Api;

namespace Zentra.Service.Implementation.Api.Specifications;

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
