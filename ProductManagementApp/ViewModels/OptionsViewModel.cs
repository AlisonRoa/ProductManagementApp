using ProductManagementApp.DTO;
using ProductManagementApp.Models;
using ProductManagementApp.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProductManagementApp.ViewModels
{
    public class OptionsViewModel : INotifyPropertyChanged
    {
        private readonly IProductsService _service;
        public ProductListItem Product { get; }
        public ObservableCollection<OptionItem> Options { get; } = new ObservableCollection<OptionItem>();
        public ObservableCollection<StatusItem> Statuses { get; } = new ObservableCollection<StatusItem>();

        private OptionItem _selected;
        public OptionItem Selected { get => _selected; set { _selected = value; OnPropertyChanged(); } }
        public ICommand AddCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CloseCommand { get; }

        public OptionsViewModel(IProductsService service, ProductListItem product)
        {
            _service = service;
            Product = product;

            AddCommand = new RelayCommand(_ => AddNew());
            SaveCommand = new RelayCommand(async _ => await SaveAsync());
            CloseCommand = new RelayCommand(w => (w as System.Windows.Window)?.Close());

            _ = LoadAsync();
        }

        private async Task LoadAsync()
        {
            Statuses.Clear();
            foreach (var s in await _service.GetStatusesAsync()) Statuses.Add(s);

            Options.Clear();
            foreach (var o in await _service.GetOptionsByProductAsync(Product.Id)) Options.Add(o);
        }

        private void AddNew()
        {
            var draft = new OptionItem
            {
                OptionCode = "OPT-NEW",
                OptionName = "",
                StatusId = Statuses.Count > 0 ? Statuses[0].Id : 0,
                ProductsId = Product.Id
            };
            Options.Insert(0, draft);
            Selected = draft;
        }

        private async Task SaveAsync()
        {
            if (Selected == null) return;
            var saved = await _service.SaveOptionAsync(Selected);
            await LoadAsync();
            // volver a seleccionar el guardado
            foreach (var o in Options) if (o.Id == saved.Id) { Selected = o; break; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string n = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}
