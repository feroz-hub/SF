using System.Windows;
using HCL.CS.DemoClientWpfApp.ViewModel;

namespace HCL.CS.DemoClientWpfApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow app = new MainWindow();
            MainWindowViewModel context = new MainWindowViewModel();
            app.DataContext = context;
            app.ShowDialog();
            base.OnStartup(e);
        }
    }
}


