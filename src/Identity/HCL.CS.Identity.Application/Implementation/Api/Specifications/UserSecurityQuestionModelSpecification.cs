using DomainValidation.Interfaces.Specification;
using DomainValidation.Validation;
using HCL.CS.Domain;
using HCL.CS.Domain.Configurations.Api;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.Domain.Models.Api;
using HCL.CS.DomainServices.UnitOfWork.Api;
using HCL.CS.DomainServices.Wrappers;
using HCL.CS.Service.Implementation.Api.Validators;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Implementation.Endpoint.Validators;

namespace HCL.CS.Service.Implementation.Api.Specifications;

internal sealed class UserSecurityQuestionModelSpecification : BaseDomainModelValidator<UserSecurityQuestionModel>
{
    internal UserSecurityQuestionModelSpecification(CrudMode crudMode, UserConfig userConfig,
        UserManagerWrapper<Users> userManager, IUserManagementUnitOfWork userManagementUnitOfWork)
    {
        Add("CheckUserSecurityQuestionModel", new Rule<UserSecurityQuestionModel>(
            new IsNotNull<UserSecurityQuestionModel>(model => model),
            ApiErrorCodes.InvalidUserSecurityQuestionModel));
        Add("CheckValidUserId", new Rule<UserSecurityQuestionModel>(
            new IsValidIdentifier<UserSecurityQuestionModel>(model => model.UserId),
            ApiErrorCodes.InvalidUserIdForSecurityQuestion));
        Add("CheckValidSecurityQuestionId", new Rule<UserSecurityQuestionModel>(
            new IsValidIdentifier<UserSecurityQuestionModel>(model => model.SecurityQuestionId),
            ApiErrorCodes.InvalidSecurityQuestionId));
        Add("CheckUserSecurityQuestionAnswer", new Rule<UserSecurityQuestionModel>(
            new IsNotNull<UserSecurityQuestionModel>(model => model.Answer),
            ApiErrorCodes.UserSecurityAnswerIsRequired));
        Add("CheckUserSecurityQuestionAnswerLength", new Rule<UserSecurityQuestionModel>(
            new CheckLengthRestrictions<UserSecurityQuestionModel>(
                model => model.Answer,
                model => userConfig.MinSecurityAnswersLength,
                model => "<"),
            ApiErrorCodes.InvalidLengthForUserSecurityAnswer));
        Add("CheckUserSecurityQuestionCreatedByLength", new Rule<UserSecurityQuestionModel>(
            new IsValid255CharLength<UserSecurityQuestionModel>(model => model.CreatedBy),
            ApiErrorCodes.CreatedByTooLong));

        Add("CheckUserExist", new Rule<UserSecurityQuestionModel>(
            new CheckUserExist(userManager),
            ApiErrorCodes.InvalidUserId));

        Add("CheckSecurityQuestionExist", new Rule<UserSecurityQuestionModel>(
            new CheckSecurityQuestionExist(userManagementUnitOfWork),
            ApiErrorCodes.InvalidSecurityQuestionId));

        if (crudMode == CrudMode.Update)
            Add("CheckUserSecurityQuestionModifiedByLength", new Rule<UserSecurityQuestionModel>(
                new IsValid255CharLength<UserSecurityQuestionModel>(model => model.ModifiedBy),
                ApiErrorCodes.ModifiedByTooLong));
    }

    internal class AnswerLengthCheck(UserConfig userConfig) : ISpecification<UserSecurityQuestionModel>
    {
        public bool IsSatisfiedBy(UserSecurityQuestionModel entity)
        {
            if (entity.Answer.Length < userConfig.MinSecurityAnswersLength) return false;

            return true;
        }
    }

    private class CheckUserExist : ISpecification<UserSecurityQuestionModel>
    {
        private readonly UserManagerWrapper<Users> userManager;

        internal CheckUserExist(UserManagerWrapper<Users> userManager)
        {
            this.userManager = userManager;
        }

        public bool IsSatisfiedBy(UserSecurityQuestionModel model)
        {
            var userExist = userManager.FindByIdAsync(model.UserId.ToString()).GetAwaiter().GetResult();
            return userExist != null;
        }
    }

    private class CheckSecurityQuestionExist : ISpecification<UserSecurityQuestionModel>
    {
        private readonly IUserManagementUnitOfWork userManagementUnitOfWork;

        internal CheckSecurityQuestionExist(IUserManagementUnitOfWork userManagementUnitOfWork)
        {
            this.userManagementUnitOfWork = userManagementUnitOfWork;
        }

        public bool IsSatisfiedBy(UserSecurityQuestionModel model)
        {
            var securityQuestions = userManagementUnitOfWork.SecurityQuestionsRepository
                .GetAsync(p => p.Id == model.SecurityQuestionId).GetAwaiter().GetResult();

            return securityQuestions.ContainsAny();
        }
    }
}
