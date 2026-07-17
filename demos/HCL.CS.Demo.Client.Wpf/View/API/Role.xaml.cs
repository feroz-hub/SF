using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HCL.CS.DemoClientWpfApp.Constants;
using HCL.CS.DemoClientWpfApp.Services;
using HCL.CS.DemoClientWpfApp.ViewModel;
using Newtonsoft.Json;
using HCL.CS.DemoClientWpfApp.Components;
using HCL.CS.DemoClientWpfApp.DomainModel;

namespace HCL.CS.DemoClientWpfApp.View
{
    public partial class Role : UserControl
    {
        public Role()
        {
            InitializeComponent();
        }

        public RoleModel RoleModel { get; set; }
        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            DataGridRow row = FindVisualParent<DataGridRow>(sender as Expander);
            row.DetailsVisibility = System.Windows.Visibility.Visible;

            RoleModel = row.DataContext as RoleModel;
            Mediator.Notify("RoleScreen", "");
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            DataGridRow row = FindVisualParent<DataGridRow>(sender as Expander);
            row.DetailsVisibility = System.Windows.Visibility.Collapsed;
        }
        public T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindVisualParent<T>(parentObject);
        }
    }
}


