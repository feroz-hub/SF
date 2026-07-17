using System.Collections.Generic;
using System.ComponentModel;
using HCL.CS.DemoClientWpfApp.DomainModel;
using static HCL.CS.DemoClientWpfApp.DomainModel.AllowedScopesParserModel;

namespace HCL.CS.DemoClientWpfApp.ViewModel
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
    }
    public static class Global
    {
        public static string PropertyUserName { get; set; }
        public static string PropertyPassword { get; set; }
        public static string PropertyTabName { get; set; }
        public static string AccessToken { get; set; }
        public static string IdToken { get; set; }
        public static string RefreshToken { get; set; }
        public static ApiScopesModel ApiScopeGetData { get; set; }
        public static ApiResourcesModel ApiResourcesGetData { get; set; }
        public static ApiResourceClaimsModel ApiResourceClaimsGetData { get; set; }
        public static ApiScopeClaimsModel ApiScopeClaimsGetData { get; set; }
        public static AllowedScopesParserModel.IdentityResourcesModel IdentityResourcesGetdata { get; set; }
        public static AllowedScopesParserModel.IdentityClaimsModel IdentityResourcesClaimGetdata { get; set; }
        public static RoleModel RoleModelGetData { get; set; }
        public static ClientsModel ClientsGetdata  { get; set; }
        public static List<string> UserRoleGetdata { get; set; }
        public static UserDisplayModel DisplayModelGetdata { get; set; }
        public static List<RoleModel> RolesGetdata { get; set; }
        public static List<RoleClaimModel> RoleClaimModelGetData { get; set; }
        public static List<UserClaimModel> UserClaimModelGetData { get; set; }
        public static string PopupMethodName { get; set; }
    }
}


