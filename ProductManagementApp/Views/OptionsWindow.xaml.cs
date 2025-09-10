// Views/OptionsWindow.xaml.cs
using ProductManagementApp.Services;
using ProductManagementApp.ViewModels;
using ProductManagementApp.Models;
using System.Windows;

namespace ProductManagementApp.Views
{
    public partial class OptionsWindow : Window
    {
        public OptionsWindow(IProductsService service, ProductListItem product)
        {
            InitializeComponent();
            DataContext = new OptionsViewModel(service, product);
        }
    }
}