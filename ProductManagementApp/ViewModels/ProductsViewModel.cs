using ProductManagementApp.DTO;
using ProductManagementApp.Models;
using ProductManagementApp.Services;
using ProductManagementApp.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using ProductManagementApp.Common;

namespace ProductManagementApp.ViewModels
{
    public class ProductsViewModel : INotifyPropertyChanged
    {
        private readonly IProductsService _service;
        private readonly ICollectionView _view;

        public IProductsService Service => _service;

        public ProductsViewModel(IProductsService service)
        {
            _service = service;
            _view = CollectionViewSource.GetDefaultView(AllProducts);
            _view.Filter = Filter;
        }

        private void OpenOptions(ProductListItem product)
        {
            var win = new OptionsWindow(_service, product);
            win.Owner = System.Windows.Application.Current.MainWindow;
            win.ShowDialog();
        }

        // Datos para la vista
        public ObservableCollection<ProductListItem> AllProducts { get; } =
            new ObservableCollection<ProductListItem>();
        public ObservableCollection<StatusItem> Statuses { get; } =
            new ObservableCollection<StatusItem>();

        // Filtros 
        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; OnFilterChanged(); }
        }

        private StatusItem _selectedStatus;
        public StatusItem SelectedStatus
        {
            get { return _selectedStatus; }
            set
            {
                _selectedStatus = value ?? new StatusItem { Id = 0, Name = "Todos" };
                OnFilterChanged();
                OnPropertyChanged();
            }
        }

        // #region Paginación (client-side)
        public int PageSize { get; } = 10;

        private int _currentPage = 1;
        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = Math.Max(1, Math.Min(value, TotalPages));
                OnPropertyChanged(nameof(PageItems));
                OnPropertyChanged(nameof(PageInfo));
                UpdatePageNumbers();
            }
        }

        public int TotalPages
        {
            get { return Math.Max(1, (int)Math.Ceiling(_view.Cast<object>().Count() / (double)PageSize)); }
        }

        public ObservableCollection<int> PageNumbers { get; } =
            new ObservableCollection<int>();

        public string PageInfo
        {
            get { return string.Format("Página {0} de {1} — {2} resultados", CurrentPage, TotalPages, _view.Cast<object>().Count()); }
        }

        public System.Collections.Generic.IEnumerable<ProductListItem> PageItems
        {
            get { return _view.Cast<ProductListItem>().Skip((CurrentPage - 1) * PageSize).Take(PageSize); }
        }

        public ICommand PrevPageCommand { get { return new RelayCommand(_ => CurrentPage--); } }
        public ICommand NextPageCommand { get { return new RelayCommand(_ => CurrentPage++); } }
        public ICommand GoToPageCommand { get { return new RelayCommand(p => { if (p is int) CurrentPage = (int)p; }); } }
        // #endregion

        // region Carga inicial
        public async Task LoadAsync()
        {
            // ===== Estados =====
            Statuses.Clear();
            var states = await _service.GetStatusesAsync();

            // Si el repo no trajo "Todos", lo insertamos al inicio
            if (!states.Any(s => string.Equals(s.Name, "Todos", StringComparison.OrdinalIgnoreCase)))
                Statuses.Add(new StatusItem { Id = 0, Name = "Todos" });

            foreach (var st in states) Statuses.Add(st);

            // Selecciona "Todos" si existe; si no, el primero disponible
            SelectedStatus = Statuses.FirstOrDefault(s => string.Equals(s.Name, "Todos", StringComparison.OrdinalIgnoreCase))
                             ?? Statuses.FirstOrDefault();

            // ===== Productos =====
            AllProducts.Clear();
            var prods = await _service.GetProductsAsync();
            foreach (var p in prods) AllProducts.Add(p);

            _view.Refresh();
            CurrentPage = 1;
            UpdatePageNumbers();
            OnPropertyChanged(nameof(PageItems));
            OnPropertyChanged(nameof(PageInfo));
        }
        // endregion

        // #region Filtrado y paginación
        private bool Filter(object obj)
        {
            var p = obj as ProductListItem;
            if (p == null) return false;

            // Si no hay estado seleccionado, no filtramos por estado
            var sel = SelectedStatus?.Name;

            if (!string.IsNullOrWhiteSpace(sel) &&
                !string.Equals(sel, "Todos", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.Equals(p.StatusName ?? "", sel, StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            // Texto
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var q = SearchText.Trim().ToLowerInvariant();
                bool hit =
                    (!string.IsNullOrEmpty(p.ProductCode) && p.ProductCode.ToLowerInvariant().Contains(q)) ||
                    (!string.IsNullOrEmpty(p.ProductName) && p.ProductName.ToLowerInvariant().Contains(q)) ||
                    (!string.IsNullOrEmpty(p.SupplierName) && p.SupplierName.ToLowerInvariant().Contains(q));
                if (!hit) return false;
            }

            return true;
        }

        private void OnFilterChanged()
        {
            try
            {
                _view.Refresh();
            }
            catch { }

            if (CurrentPage > TotalPages) CurrentPage = TotalPages;
            if (CurrentPage < 1) CurrentPage = 1;

            OnPropertyChanged(nameof(PageItems));
            OnPropertyChanged(nameof(PageInfo));
            UpdatePageNumbers();
        }


        private void UpdatePageNumbers()
        {
            PageNumbers.Clear();
            int total = TotalPages;
            int start = Math.Max(1, CurrentPage - 2);
            int end = Math.Min(total, start + 4);
            for (int i = start; i <= end; i++) PageNumbers.Add(i);
            OnPropertyChanged(nameof(PageNumbers));
        }

        // ====== INotifyPropertyChanged ======
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(prop));
        }
    }
    // #endregion

}
