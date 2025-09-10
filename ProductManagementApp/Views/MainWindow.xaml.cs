using System.Windows;
using ProductManagementApp.Repositories;
using ProductManagementApp.Services;
using ProductManagementApp.ViewModels;
using ProductManagementApp.Common;

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
    }
}
