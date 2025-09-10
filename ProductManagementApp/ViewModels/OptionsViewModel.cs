using System;
using ProductManagementApp.DTO;
using ProductManagementApp.Models;
using ProductManagementApp.Services;
using ProductManagementApp.Common;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ProductManagementApp.ViewModels
{
    public class OptionsViewModel : INotifyPropertyChanged
    {
        private readonly IProductsService _service;

        public ProductListItem Product { get; }

        public bool OptionsEnabled { get; }
        public bool OptionsReadOnly => !OptionsEnabled;

        public ObservableCollection<OptionItem> Options { get; } = new ObservableCollection<OptionItem>();
        public ObservableCollection<StatusItem> Statuses { get; } = new ObservableCollection<StatusItem>();

        
        private OptionItem _selected;
        public OptionItem Selected
        {
            get => _selected;
            set { _selected = value; OnPropertyChanged(); }
        }

        public ICommand AddCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand CloseCommand { get; }
        
        public OptionsViewModel(IProductsService service, ProductListItem product)
        {
            _service = service;
            Product = product;

            OptionsEnabled = string.Equals(Product.StatusName, "Activo", StringComparison.OrdinalIgnoreCase)
                             || string.Equals(Product.StatusName, "Active", StringComparison.OrdinalIgnoreCase);

            AddCommand = new RelayCommand(_ => AddNew(), _ => OptionsEnabled);
            SaveCommand = new RelayCommand(async _ => await SaveAsync(), _ => Selected != null && OptionsEnabled);
            DeleteCommand = new RelayCommand(async _ => await DeleteAsync(), _ => Selected?.Id > 0 && OptionsEnabled);
            CloseCommand = new RelayCommand(w => (w as Window)?.Close());

            _ = LoadAsync();
        }


        private void RaiseCommands()
        {
            (AddCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        private async Task LoadAsync()
        {
            Statuses.Clear();
            foreach (var s in await _service.GetStatusesAsync())
                Statuses.Add(s);

            Options.Clear();
            foreach (var o in await _service.GetOptionsByProductAsync(Product.Id))
                Options.Add(o);
        }

        private void AddNew()
        {
            var draft = new OptionItem
            {
                Id = 0,
                OptionCode = "OTP-",
                OptionName = "",
                StatusId = Statuses.Count > 0 ? Statuses[0].Id : 0,
                ProductsId = Product.Id
            };
            Options.Insert(0, draft);
            Selected = draft;
        }

        private bool Validate(OptionItem item, out string error)
        {
            if (item == null) { error = "Nada seleccionado."; return false; }
            if (string.IsNullOrWhiteSpace(item.OptionName))
            { error = "El nombre de la opción es requerido."; return false; }

            if (string.IsNullOrWhiteSpace(item.OptionCode))
            { error = "El código de la opción es requerido."; return false; }

            if (item.StatusId <= 0)
            { error = "Debes seleccionar un estado."; return false; }

            error = null;
            return true;
        }

        private async Task SaveAsync()
        {
            if (Selected == null) return;

            if (Selected.Id == 0 &&
                (string.IsNullOrWhiteSpace(Selected.OptionCode) || Selected.OptionCode == "OTP-" || Selected.OptionCode == "OTP-NEW"))
            {
                Selected.OptionCode = await _service.GetNextOtpCodeAsync();
            }

            bool exists = await _service.OptionCodeExistsAsync(
                Selected.OptionCode,
                Selected.Id > 0 ? (int?)Selected.Id : null
            );
            if (exists)
            {
                MessageBox.Show("El código de opción ya existe en la base de datos. Cambia el código o vuelve a intentar.",
                    "Código duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!Validate(Selected, out var err))
            {
                MessageBox.Show(err, "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool wasNew = Selected.Id == 0;

            var saved = await _service.SaveOptionAsync(Selected);

            Selected.Id = saved.Id;
            Selected.OptionCode = saved.OptionCode;
            Selected.OptionName = saved.OptionName;
            Selected.StatusId = saved.StatusId;

            var st = Statuses.FirstOrDefault(x => x.Id == Selected.StatusId);
            if (st != null) Selected.StatusName = st.Name;

            OnPropertyChanged(nameof(Selected));

            MessageBox.Show(wasNew ? "Opción creada." : "Opción actualizada.",
                "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async Task DeleteAsync()
        {
            if (Selected == null || Selected.Id <= 0) return;

            if (MessageBox.Show("¿Eliminar esta opción?", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            await _service.DeleteOptionAsync(Selected.Id);
            await LoadAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string n = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));

        private string GenerateNextOptionCode()
        {
            const string prefix = "OPT-";
            int max = 0;

            foreach (var o in Options)
            {
                if (string.IsNullOrWhiteSpace(o.OptionCode)) continue;

                if (o.OptionCode.StartsWith(prefix))
                {
                    var tail = o.OptionCode.Substring(prefix.Length);
                    if (int.TryParse(tail, out int n) && n > max)
                        max = n;
                }
            }

            return $"{prefix}{(max + 1).ToString("D3")}";
        }

    }
}
