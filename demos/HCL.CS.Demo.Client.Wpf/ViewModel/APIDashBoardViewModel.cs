using HCL.CS.DemoClientWpfApp.Components;
using HCL.CS.DemoClientWpfApp.Constants;
using HCL.CS.DemoClientWpfApp.Interface;

namespace HCL.CS.DemoClientWpfApp.ViewModel
{
    internal  class APIDashBoardViewModel :BaseViewModel, IPageViewModel
    {
        ApplicationParameters ApplicationConstants = new ApplicationParameters();
        public RelayCommand APiResourceCommand { get; set; }
        public RelayCommand IdentityResourceCommand { get; set; }
        public RelayCommand CleintCommand { get; set; }
        public RelayCommand RoleCommand { get; set; }
        public RelayCommand HomeCommand { get; set; }
        public RelayCommand UserRoleCommand { get; set; }
        public APIDashBoardViewModel()
        {
            HomeCommand = new RelayCommand(param => OnHome());
            APiResourceCommand = new RelayCommand(param => GetApiResource());
            IdentityResourceCommand = new RelayCommand(param => GetIdentityResource());
            CleintCommand = new RelayCommand(param => GetClient());
            RoleCommand = new RelayCommand(param => GetRole());
            HomeCommand = new RelayCommand(param => OnHome());
            UserRoleCommand = new RelayCommand(param => UserRole());
        }
        private void OnHome()
        {
            Mediator.Notify("DashBoardScreen", "");
        }
        private void UserRole()
        {
            Mediator.Notify("UserRoleScreen", "");
        }
        private void GetApiResource()
        {
            Mediator.Notify("ApiResourceGridScreen", "");
        }
        private void GetIdentityResource()
        {
            Mediator.Notify("IdentityResourceScreen", "");
        }
        private void GetClient()
        {
            Mediator.Notify("ClientScreen", "");
        }
        private void GetRole()
        {
            Mediator.Notify("RoleScreen", "");
        }
    }
}


