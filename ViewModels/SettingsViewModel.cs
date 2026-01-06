using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using RestaurantApp.Models;
using RestaurantApp.Services;

namespace RestaurantApp.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private readonly AppSettings _settings;
        private readonly DataService _dataService;
        private readonly MainViewModel _mainViewModel;

        private int _insideTables;
        private int _outsideTables;
        private string _defaultPrinterName = string.Empty;
        private string _networkPrinterIP = "192.168.1.100";
        private int _networkPrinterPort = 9100;
        private string _selectedLanguage = "English";
        private ObservableCollection<string> _availablePrinters = new();
        private ObservableCollection<string> _availableLanguages = new() { "English", "Turkish" };

        private ObservableCollection<MenuItem> _menuItems = new();
        private ObservableCollection<Table> _tables = new();
        private MenuItem? _selectedMenuItem;
        private Table? _selectedTable;

        public int InsideTables
        {
            get => _insideTables;
            set => SetProperty(ref _insideTables, value);
        }

        public int OutsideTables
        {
            get => _outsideTables;
            set => SetProperty(ref _outsideTables, value);
        }

        public string DefaultPrinterName
        {
            get => _defaultPrinterName;
            set => SetProperty(ref _defaultPrinterName, value);
        }

        public string NetworkPrinterIP
        {
            get => _networkPrinterIP;
            set => SetProperty(ref _networkPrinterIP, value);
        }

        public int NetworkPrinterPort
        {
            get => _networkPrinterPort;
            set => SetProperty(ref _networkPrinterPort, value);
        }

        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set => SetProperty(ref _selectedLanguage, value);
        }

        public ObservableCollection<string> AvailablePrinters
        {
            get => _availablePrinters;
            set => SetProperty(ref _availablePrinters, value);
        }

        public ObservableCollection<string> AvailableLanguages
        {
            get => _availableLanguages;
            set => SetProperty(ref _availableLanguages, value);
        }

        public ObservableCollection<MenuItem> MenuItems
        {
            get => _menuItems;
            set => SetProperty(ref _menuItems, value);
        }

        public ObservableCollection<Table> Tables
        {
            get => _tables;
            set => SetProperty(ref _tables, value);
        }

        public MenuItem? SelectedMenuItem
        {
            get => _selectedMenuItem;
            set => SetProperty(ref _selectedMenuItem, value);
        }

        public Table? SelectedTable
        {
            get => _selectedTable;
            set => SetProperty(ref _selectedTable, value);
        }

        public LocalizationService Localization => _mainViewModel.Localization;

        public string LocalizedSettingsTitle => Localization.GetString("SettingsTitle");
        public string LocalizedGeneralTab => Localization.GetString("GeneralTab");
        public string LocalizedMenuManagementTab => Localization.GetString("MenuManagementTab");
        public string LocalizedTablesTab => Localization.GetString("TablesTab");
        public string LocalizedLanguageLabel => Localization.GetString("LanguageLabel");
        public string LocalizedPrinterConfigLabel => Localization.GetString("PrinterConfigLabel");
        public string LocalizedNetworkPrinterIPLabel => Localization.GetString("NetworkPrinterIPLabel");
        public string LocalizedNetworkPrinterPortLabel => Localization.GetString("NetworkPrinterPortLabel");
        public string LocalizedMenuItemsLabel => Localization.GetString("MenuItemsLabel");
        public string LocalizedEditItemLabel => Localization.GetString("EditItemLabel");
        public string LocalizedNameLabel => Localization.GetString("NameLabel");
        public string LocalizedCategoryLabel => Localization.GetString("CategoryLabel");
        public string LocalizedPriceLabel => Localization.GetString("PriceLabel");
        public string LocalizedToppingsLabel => Localization.GetString("ToppingsLabel");
        public string LocalizedAddToppingButton => Localization.GetString("AddToppingButton");
        public string LocalizedTablesLabel => Localization.GetString("TablesLabel");
        public string LocalizedEditTableLabel => Localization.GetString("EditTableLabel");
        public string LocalizedDisplayNameLabel => Localization.GetString("DisplayNameLabel");
        public string LocalizedTableNumberLabel => Localization.GetString("TableNumberLabel");
        public string LocalizedLocationLabel => Localization.GetString("LocationLabel");
        public string LocalizedSaveAllSettingsButton => Localization.GetString("SaveAllSettingsButton");

        public System.Windows.Input.ICommand SaveCommand { get; }
        public System.Windows.Input.ICommand AddMenuItemCommand { get; }
        public System.Windows.Input.ICommand DeleteMenuItemCommand { get; }
        public System.Windows.Input.ICommand AddToppingCommand { get; }
        public System.Windows.Input.ICommand DeleteToppingCommand { get; }
        public System.Windows.Input.ICommand AddTableCommand { get; }
        public System.Windows.Input.ICommand DeleteTableCommand { get; }

        public SettingsViewModel(AppSettings settings, DataService dataService, MainViewModel mainViewModel)
        {
            _settings = settings;
            _dataService = dataService;
            _mainViewModel = mainViewModel;
            SaveCommand = new RelayCommand(SaveSettings);
            AddMenuItemCommand = new RelayCommand(AddMenuItem);
            DeleteMenuItemCommand = new RelayCommand(DeleteMenuItem);
            AddToppingCommand = new RelayCommand(AddTopping);
            DeleteToppingCommand = new RelayCommand(DeleteTopping);
            AddTableCommand = new RelayCommand(AddTable);
            DeleteTableCommand = new RelayCommand(DeleteTable);

            InsideTables = settings.InsideTables;
            OutsideTables = settings.OutsideTables;
            DefaultPrinterName = settings.DefaultPrinterName ?? string.Empty;
            NetworkPrinterIP = settings.NetworkPrinterIP ?? "192.168.1.100";
            NetworkPrinterPort = settings.NetworkPrinterPort > 0 ? settings.NetworkPrinterPort : 9100;
            SelectedLanguage = settings.Language ?? "English";

            foreach (var item in settings.MenuItems)
                MenuItems.Add(item);

            foreach (var table in settings.Tables)
                Tables.Add(table);

            LoadPrinters();
        }

        private void AddMenuItem(object? obj)
        {
            var newItem = new MenuItem { Name = "New Item", Category = "Main", Price = 0 };
            MenuItems.Add(newItem);
            SelectedMenuItem = newItem;
        }

        private void DeleteMenuItem(object? obj)
        {
            if (obj is MenuItem item)
            {
                MenuItems.Remove(item);
                if (SelectedMenuItem == item) SelectedMenuItem = null;
            }
        }

        private void AddTopping(object? obj)
        {
            if (SelectedMenuItem != null)
            {
                SelectedMenuItem.AvailableToppings.Add(new Topping { Name = "New Topping", Price = 0 });
                OnPropertyChanged(nameof(SelectedMenuItem));
            }
        }

        private void DeleteTopping(object? obj)
        {
            if (obj is Topping topping && SelectedMenuItem != null)
            {
                SelectedMenuItem.AvailableToppings.Remove(topping);
                OnPropertyChanged(nameof(SelectedMenuItem));
            }
        }

        private void AddTable(object? obj)
        {
            int nextNum = Tables.Count > 0 ? Tables.Max(t => t.Number) + 1 : 1;
            var newTable = new Table { Number = nextNum, Location = "inside", DisplayName = $"Table {nextNum}" };
            Tables.Add(newTable);
            SelectedTable = newTable;
        }

        private void DeleteTable(object? obj)
        {
            if (obj is Table table)
            {
                Tables.Remove(table);
                if (SelectedTable == table) SelectedTable = null;
            }
        }

        private void LoadPrinters()
        {
            try
            {
                PrinterService printerService = new();
                var printers = printerService.GetAvailablePrinters();
                
                AvailablePrinters.Clear();
                AvailablePrinters.Add("Default Printer");
                
                foreach (var printer in printers)
                {
                    AvailablePrinters.Add(printer);
                }

                // If currently selected printer is not in the list anymore, reset to Default
                if (string.IsNullOrEmpty(DefaultPrinterName) || !AvailablePrinters.Contains(DefaultPrinterName))
                {
                    DefaultPrinterName = "Default Printer";
                }
            }
            catch
            {
                // If PrinterService fails, just add default
                AvailablePrinters.Clear();
                AvailablePrinters.Add("Default Printer");
                DefaultPrinterName = "Default Printer";
            }
        }

        private void SaveSettings(object? obj)
        {
            _settings.InsideTables = InsideTables;
            _settings.OutsideTables = OutsideTables;
            _settings.TotalTables = Tables.Count;
            _settings.DefaultPrinterName = DefaultPrinterName;
            _settings.NetworkPrinterIP = NetworkPrinterIP;
            _settings.NetworkPrinterPort = NetworkPrinterPort;
            _settings.Language = SelectedLanguage;
            _settings.MenuItems = MenuItems.ToList();
            _settings.Tables = Tables.ToList();

            _dataService.SaveSettings(_settings);
            _mainViewModel.UpdateLanguage(SelectedLanguage);
            System.Windows.MessageBox.Show("Settings saved successfully!");
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
                return false;

            field = newValue;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}
