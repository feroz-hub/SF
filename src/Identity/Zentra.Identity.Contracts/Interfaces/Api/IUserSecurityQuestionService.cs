using Zentra.Domain;
using Zentra.Domain.Models.Api;

namespace Zentra.Service.Interfaces.Interfaces.Api;

public partial interface IUserAccountService
{
    Task<FrameworkResult> AddSecurityQuestionAsync(SecurityQuestionModel securityQuestionModel);

    Task<FrameworkResult> UpdateSecurityQuestionAsync(SecurityQuestionModel securityQuestionModel);

    Task<FrameworkResult> DeleteSecurityQuestionAsync(Guid securityQuestionId);

    Task<IList<SecurityQuestionModel>> GetAllSecurityQuestionsAsync();

    Task<FrameworkResult> AddUserSecurityQuestionAsync(UserSecurityQuestionModel userSecurityQuestionModel);

    Task<FrameworkResult> AddUserSecurityQuestionAsync(IList<UserSecurityQuestionModel> userSecurityQuestionModels);

    Task<FrameworkResult> UpdateUserSecurityQuestionAsync(UserSecurityQuestionModel userSecurityQuestionModel);

    Task<FrameworkResult> DeleteUserSecurityQuestionAsync(UserSecurityQuestionModel userSecurityQuestionModel);

    Task<FrameworkResult> DeleteUserSecurityQuestionAsync(IList<UserSecurityQuestionModel> userSecurityQuestionModels);

    Task<IList<UserSecurityQuestionModel>> GetUserSecurityQuestionsAsync(Guid userId);
}
