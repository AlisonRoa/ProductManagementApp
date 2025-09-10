using System;
using ProductManagementApp.Common;
using ProductManagementApp.Models;
using ProductManagementApp.Repositories;
using ProductManagementApp.Services;
using ProductManagementApp.ViewModels;
using System.Windows;
using ProductManagementApp.Views;

namespace ProductManagementApp
{
    public partial class MainWindow : Window
    {
        public ProductsViewModel VM { get; }

        public MainWindow()
        {
            InitializeComponent();

            var repo = new ProductsRepository();
            var service = new ProductsService(repo);

            VM = new ProductsViewModel(service);
            DataContext = VM;

            Loaded += async (_, __) => await VM.LoadAsync();
        }

        private void OpenOptions_Click(object sender, RoutedEventArgs e)
        {
            var fe = sender as FrameworkElement;
            var product = fe?.DataContext as ProductListItem;
            if (product == null) return;

            // Bloqueo defensivo
            if (string.Equals(product.StatusName, "Inactivo", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(product.StatusName, "Inactive", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Este producto está inactivo. No se pueden gestionar opciones.",
                    "Producto inactivo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var vmMain = DataContext as ProductsViewModel;
            var win = new OptionsWindow(vmMain.Service, product);
            win.Owner = System.Windows.Application.Current.MainWindow;
            win.ShowDialog();
        }
    }
}
