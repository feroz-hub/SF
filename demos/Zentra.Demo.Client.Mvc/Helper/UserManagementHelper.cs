using Zentra.DemoClientMvc.Extension;
using Zentra.DemoClientMvc.Interface;
using Zentra.Domain.Constants;
using Zentra.Domain.Models.Api;

namespace Zentra.DemoClientMvc.Helper;

public class UserManagementHelper(IHttpService httpService)
{
    public IList<SecurityQuestionModel> LoadSecurityQuestionCombo()
    {
        var securityQuestionList = httpService.PostAsync<IList<SecurityQuestionModel>>(
            ApiRoutePathConstants.GetAllSecurityQuestions,
            string.Empty).GetAwaiter().GetResult();
        if (!securityQuestionList.ContainsAny()) return null;
        securityQuestionList.Insert(0, new SecurityQuestionModel
        {
            Question = "----Select----"
            //Id = new System.Guid()
        });

        return securityQuestionList;
    }
}
