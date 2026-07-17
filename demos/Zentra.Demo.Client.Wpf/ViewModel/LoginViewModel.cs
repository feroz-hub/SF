using Zentra.DemoClientWpfApp.Components;
using Zentra.DemoClientWpfApp.Interface;

namespace Zentra.DemoClientWpfApp.ViewModel
{
    internal class LoginViewModel : BaseViewModel, IPageViewModel
    {
        public RelayCommand LoginCommand { get; set; }

        public LoginViewModel()
        {
        }

    }
}


