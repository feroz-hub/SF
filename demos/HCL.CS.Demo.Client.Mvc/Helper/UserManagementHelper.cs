using HCL.CS.DemoClientMvc.Extension;
using HCL.CS.DemoClientMvc.Interface;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Models.Api;

namespace HCL.CS.DemoClientMvc.Helper;

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
