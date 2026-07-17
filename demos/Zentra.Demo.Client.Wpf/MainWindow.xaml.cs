using System.Windows;

namespace Zentra.DemoClientWpfApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
             this.Height = SystemParameters.PrimaryScreenHeight * 0.95;
            this.Width = SystemParameters.PrimaryScreenWidth * 0.95;
        }
    }
}


