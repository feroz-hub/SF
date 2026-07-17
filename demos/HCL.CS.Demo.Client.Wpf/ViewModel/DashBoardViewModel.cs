using HCL.CS.DemoClientWpfApp.Components;
using HCL.CS.DemoClientWpfApp.Interface;

namespace HCL.CS.DemoClientWpfApp.ViewModel
{
    internal  class DashBoardViewModel :BaseViewModel, IPageViewModel
    {
        public RelayCommand UserProfileCommand { get; set; }
        public RelayCommand PasswordChangeCommand { get; set; }
        public RelayCommand TwoFactorCommand { get; set; }

        public DashBoardViewModel()
        {
            UserProfileCommand = new RelayCommand(param => GoToUserProfile());
            PasswordChangeCommand = new RelayCommand(param => GoToPasswordChange());
            TwoFactorCommand = new RelayCommand(param => GoToTwoFactor());
        }

        public void GoToUserProfile()
        {
            Mediator.Notify("UpdateProfileScreen", "");
        }
        public void GoToPasswordChange()
        {
            Mediator.Notify("ChangePasswordScreen", "");
        }
        public void GoToTwoFactor()
        {
            Mediator.Notify("ManageAccountScreen", "");

        }
    }
}


