using HCL.CS.DemoClientWpfApp.Components;
using HCL.CS.DemoClientWpfApp.Interface;

namespace HCL.CS.DemoClientWpfApp.ViewModel
{
    internal class LoginViewModel : BaseViewModel, IPageViewModel
    {
        public RelayCommand LoginCommand { get; set; }

        public LoginViewModel()
        {
        }

    }
}


