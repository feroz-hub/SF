using DomainValidation.Interfaces.Specification;
using DomainValidation.Validation;
using HCL.CS.Domain;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.Domain.Models.Api;
using HCL.CS.DomainServices.UnitOfWork.Api;
using HCL.CS.Service.Implementation.Api.Validators;

namespace HCL.CS.Service.Implementation.Api.Specifications;

internal sealed class SecurityQuestionModelSpecification : BaseDomainModelValidator<SecurityQuestionModel>
{
    internal SecurityQuestionModelSpecification(CrudMode crudMode, SecurityQuestionModel securityQuestionModel,
        IUserManagementUnitOfWork userManagementUnitOfWork)
    {
        Add("CheckValidQuestion", new Rule<SecurityQuestionModel>(
            new IsNotNull<SecurityQuestionModel>(model => model.Question),
            ApiErrorCodes.InvalidSecurityQuestion));
        Add("CheckValidQuestionLength", new Rule<SecurityQuestionModel>(
            new IsValid255CharLength<SecurityQuestionModel>(model => model.Question),
            ApiErrorCodes.SecurityQuestionTooLong));
        Add("CheckCreatedByLength", new Rule<SecurityQuestionModel>(
            new IsValid255CharLength<SecurityQuestionModel>(model => model.CreatedBy),
            ApiErrorCodes.CreatedByTooLong));

        switch (crudMode)
        {
            case CrudMode.Add:
                Add("CheckSecurityQuestionEntry", new Rule<SecurityQuestionModel>(
                    new CheckSecurityQuestionEntry(securityQuestionModel, userManagementUnitOfWork),
                    ApiErrorCodes.SecurityQuestionAlreadyExists));
                break;
            case CrudMode.Update:
                Add("CheckValidSecurityQuestionId", new Rule<SecurityQuestionModel>(
                    new IsValidIdentifier<SecurityQuestionModel>(model => model.Id),
                    ApiErrorCodes.InvalidSecurityQuestionId));
                Add("CheckModifiedByLength", new Rule<SecurityQuestionModel>(
                    new IsValid255CharLength<SecurityQuestionModel>(model => model.ModifiedBy),
                    ApiErrorCodes.ModifiedByTooLong));

                Add("CheckSecurityQuestionEntry", new Rule<SecurityQuestionModel>(
                    new CheckSecurityQuestionEntry(securityQuestionModel, userManagementUnitOfWork),
                    ApiErrorCodes.SecurityQuestionAlreadyExists));
                break;
            case CrudMode.Delete:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(crudMode), crudMode, null);
        }
    }

    private class CheckSecurityQuestionEntry : ISpecification<SecurityQuestionModel>
    {
        private readonly SecurityQuestionModel securityQuestionModel1;
        private readonly IUserManagementUnitOfWork userManagementUnitOfWork;

        internal CheckSecurityQuestionEntry(SecurityQuestionModel securityQuestionModel,
            IUserManagementUnitOfWork userManagementUnitOfWork)
        {
            this.userManagementUnitOfWork = userManagementUnitOfWork;
            securityQuestionModel1 = securityQuestionModel;
        }

        public bool IsSatisfiedBy(SecurityQuestionModel model)
        {
            var duplicateExists = userManagementUnitOfWork.SecurityQuestionsRepository
                .DuplicateExistsAsync(x => x.Question == securityQuestionModel1.Question).GetAwaiter().GetResult();
            return !duplicateExists;
        }
    }
}
