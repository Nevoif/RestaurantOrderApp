using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using RestaurantApp.Models;
using RestaurantApp.Services;
using RestaurantApp.Views;

namespace RestaurantApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly DataService _dataService = new();
        private readonly PrinterService _printerService = new();
        private readonly LocalizationService _localization = LocalizationService.Instance;

        private AppSettings _settings = null!;
        private TableOrder? _selectedTableOrder;
        private MenuItem? _selectedMenuItem;
        private ObservableCollection<Topping> _selectedToppings = new();
        private int _selectedQuantity = 1;
        private string _currencySymbol = "$";
        private bool _selectAllItems = false;
        private string _restaurantName = "Restaurant";

        public ObservableCollection<Table> Tables { get; } = new();
        public ObservableCollection<TableOrder> ActiveOrders { get; } = new();
        public ObservableCollection<MenuItem> MenuItems { get; } = new();
        public ObservableCollection<MenuItem> DrinkItems { get; } = new();
        public ObservableCollection<MenuItem> MainItems { get; } = new();
        public ObservableCollection<OrderItem> CurrentOrderItems { get; } = new();

        public AppSettings Settings
        {
            get => _settings;
            set => SetProperty(ref _settings, value);
        }

        public TableOrder? SelectedTableOrder
        {
            get => _selectedTableOrder;
            set
            {
                if (SetProperty(ref _selectedTableOrder, value))
                {
                    CurrentOrderItems.Clear();
                    SelectAllItems = false;
                    if (value != null)
                    {
                        foreach (var item in value.OrderItems)
                        {
                            // Subscribe to item selection changes
                            item.PropertyChanged += (s, e) =>
                            {
                                if (e.PropertyName == nameof(OrderItem.IsSelected))
                                {
                                    SelectAllItems = CurrentOrderItems.All(i => i.IsSelected) && CurrentOrderItems.Count > 0;
                                }
                            };
                            item.IsSelected = false;
                            CurrentOrderItems.Add(item);
                        }
                    }
                    OnPropertyChanged(nameof(CurrentOrderTotal));
                }
            }
        }

        public MenuItem? SelectedMenuItem
        {
            get => _selectedMenuItem;
            set
            {
                if (SetProperty(ref _selectedMenuItem, value))
                {
                    if (value != null)
                    {
                        foreach (var topping in value.AvailableToppings)
                            topping.IsSelected = false;
                    }
                    SelectedToppings.Clear();
                    SelectedQuantity = 1;
                }
            }
        }

        public ObservableCollection<Topping> SelectedToppings
        {
            get => _selectedToppings;
            set => SetProperty(ref _selectedToppings, value);
        }

        public int SelectedQuantity
        {
            get => _selectedQuantity;
            set => SetProperty(ref _selectedQuantity, value);
        }

        public string CurrencySymbol
        {
            get => _currencySymbol;
            set => SetProperty(ref _currencySymbol, value);
        }

        public string RestaurantName
        {
            get => _restaurantName;
            set => SetProperty(ref _restaurantName, value);
        }

        public bool SelectAllItems
        {
            get => _selectAllItems;
            set
            {
                if (SetProperty(ref _selectAllItems, value))
                {
                    // Update all items' selection state based on SelectAllItems
                    foreach (var item in CurrentOrderItems.ToList())
                    {
                        item.IsSelected = value;
                    }
                }
            }
        }

        public string CurrentOrderTotal => _localization.FormatCurrency(SelectedTableOrder?.GetTotal() ?? 0);

        public LocalizationService Localization => _localization;

        // Localization string properties for binding
        public string LocalizedTitle => _localization.GetString("Header");
        public string LocalizedSettingsButton => _localization.GetString("SettingsButton");
        public string LocalizedEarningsButton => _localization.GetString("EarningsButton");
        public string LocalizedTablesLabel => _localization.GetString("TablesLabel");
        public string LocalizedMainFoodsTab => _localization.GetString("MainFoodsTab");
        public string LocalizedDrinksTab => _localization.GetString("DrinksTab");
        public string LocalizedToppingsLabel => _localization.GetString("ToppingsLabel");
        public string LocalizedSelectedItemLabel => _localization.GetString("SelectedItemLabel");
        public string LocalizedQuantityLabel => _localization.GetString("QuantityLabel");
        public string LocalizedAddToOrderButton => _localization.GetString("AddToOrderButton");
        public string LocalizedCurrentOrderLabel => _localization.GetString("CurrentOrderLabel");
        public string LocalizedTotalLabel => _localization.GetString("TotalLabel");
        public string LocalizedPrintButton => _localization.GetString("PrintButton");
        public string LocalizedCheckoutButton => _localization.GetString("CheckoutButton");
        public string LocalizedCancelButton => _localization.GetString("CancelButton");
        public string LocalizedDeleteButton => _localization.GetString("DeleteButton");
        public string LocalizedTableOrderLabel => _localization.GetString("CurrentOrderLabel");
        public string LocalizedQtyLabel => _localization.GetString("QuantityLabel");
        public string LocalizedCheckoutCreditCard => _localization.GetString("CheckoutCreditCardButton");
        public string LocalizedCheckoutCash => _localization.GetString("CheckoutCashButton");
        public string LocalizedCheckoutPackageOrder => _localization.GetString("CheckoutPackageOrderButton");
        public string LocalizedPrintReceiptButton => _localization.GetString("PrintReceiptButton");

        public System.Windows.Input.ICommand SelectTableCommand { get; }
        public System.Windows.Input.ICommand SelectItemCommand { get; }
        public System.Windows.Input.ICommand AddToOrderCommand { get; }
        public System.Windows.Input.ICommand RemoveFromOrderCommand { get; }
        public System.Windows.Input.ICommand PrintOrderCommand { get; }
        public System.Windows.Input.ICommand PrintReceiptCommand { get; }
        public System.Windows.Input.ICommand CheckoutCreditCardCommand { get; }
        public System.Windows.Input.ICommand CheckoutCashCommand { get; }
        public System.Windows.Input.ICommand CheckoutPackageOrderCommand { get; }
        public System.Windows.Input.ICommand CancelCommand { get; }
        public System.Windows.Input.ICommand SettingsCommand { get; }
        public System.Windows.Input.ICommand EarningsCommand { get; }
        public System.Windows.Input.ICommand ToggleToppingCommand { get; }
        public System.Windows.Input.ICommand SelectAllItemsCommand { get; }
        public System.Windows.Input.ICommand ToggleOrderItemCommand { get; }

        public MainViewModel()
        {
            SelectTableCommand = new RelayCommand(SelectTable);
            SelectItemCommand = new RelayCommand(SelectItem);
            AddToOrderCommand = new RelayCommand(AddToOrder, _ => SelectedMenuItem != null && SelectedTableOrder != null);
            RemoveFromOrderCommand = new RelayCommand(RemoveFromOrder, _ => SelectedTableOrder != null);
            PrintOrderCommand = new RelayCommand(PrintOrder, _ => SelectedTableOrder != null && SelectedTableOrder.OrderItems.Any(i => !i.IsPrinted));
            PrintReceiptCommand = new RelayCommand(PrintReceipt, _ => SelectedTableOrder != null && SelectedTableOrder.OrderItems.Count > 0);
            CheckoutCreditCardCommand = new RelayCommand(o => CheckoutTableWithPayment(PaymentMethod.CreditCard), _ => SelectedTableOrder != null && ActiveOrders.Contains(SelectedTableOrder));
            CheckoutCashCommand = new RelayCommand(o => CheckoutTableWithPayment(PaymentMethod.Cash), _ => SelectedTableOrder != null && ActiveOrders.Contains(SelectedTableOrder));
            CheckoutPackageOrderCommand = new RelayCommand(o => CheckoutTableWithPayment(PaymentMethod.PackageOrder), _ => SelectedTableOrder != null && ActiveOrders.Contains(SelectedTableOrder));
            CancelCommand = new RelayCommand(CancelTable, _ => SelectedTableOrder != null && ActiveOrders.Contains(SelectedTableOrder));
            SettingsCommand = new RelayCommand(OpenSettings);
            EarningsCommand = new RelayCommand(OpenEarnings);
            ToggleToppingCommand = new RelayCommand(ToggleTopping);
            SelectAllItemsCommand = new RelayCommand(SelectAllItems_Executed);
            ToggleOrderItemCommand = new RelayCommand(ToggleOrderItem);

            LoadData();
        }

        private void LoadData()
        {
            Settings = _dataService.LoadSettings();
            
            // Configure network printer settings
            _printerService.SetNetworkPrinterIP(Settings.NetworkPrinterIP, Settings.NetworkPrinterPort);
            
            // Set restaurant name
            RestaurantName = Settings.RestaurantName ?? "Restaurant";
            
            // Set up localization
            _localization.CurrentLanguage = Settings.Language == "Turkish" 
                ? LocalizationService.Language.Turkish 
                : LocalizationService.Language.English;
            CurrencySymbol = _localization.GetCurrencySymbol();
            OnPropertyChanged(nameof(Localization));
            
            MenuItems.Clear();
            foreach (var item in Settings.MenuItems)
                MenuItems.Add(item);

            MainItems.Clear();
            foreach (var item in MenuItems.Where(m => m.Category == "Main"))
                MainItems.Add(item);

            DrinkItems.Clear();
            foreach (var item in MenuItems.Where(m => m.Category == "Drink"))
                DrinkItems.Add(item);

            InitializeTables();
            LoadOrders();
        }

        private void InitializeTables()
        {
            Tables.Clear();
            
            if (Settings.Tables != null && Settings.Tables.Count > 0)
            {
                foreach (var table in Settings.Tables)
                {
                    Tables.Add(table);
                }
            }
            else
            {
                // Fallback to count-based initialization for the first time
                for (int i = 1; i <= Settings.InsideTables; i++)
                {
                    Tables.Add(new Table { Number = i, Location = "inside", DisplayName = $"Table {i} (Inside)" });
                }
                
                for (int i = 1; i <= Settings.OutsideTables; i++)
                {
                    int tableNum = Settings.InsideTables + i;
                    Tables.Add(new Table { Number = tableNum, Location = "outside", DisplayName = $"Table {tableNum} (Outside)" });
                }
                
                // Save these as default tables
                Settings.Tables = Tables.ToList();
                _dataService.SaveSettings(Settings);
            }
        }

        private void LoadOrders()
        {
            var allOrders = _dataService.LoadOrders();
            ActiveOrders.Clear();
            
            foreach (var order in allOrders.Where(o => o.Status == OrderStatus.Active))
            {
                ActiveOrders.Add(order);
                var table = Tables.FirstOrDefault(t => t.Number == order.TableNumber);
                if (table != null)
                    table.HasActiveOrder = true;
            }
        }

        private void SelectTable(object? obj)
        {
            if (obj is int tableNum)
            {
                foreach (var table in Tables)
                {
                    table.IsSelected = (table.Number == tableNum);
                }

                var selectedTable = Tables.First(t => t.Number == tableNum);
                var existingOrder = ActiveOrders.FirstOrDefault(o => o.TableNumber == tableNum);
                SelectedTableOrder = existingOrder ?? new TableOrder 
                { 
                    TableNumber = tableNum, 
                    TableLocation = selectedTable.Location,
                    TableDisplayName = selectedTable.DisplayName
                };
            }
        }

        private void SelectItem(object? obj)
        {
            if (obj is MenuItem item)
            {
                SelectedMenuItem = item;
            }
        }

        private void ToggleTopping(object? obj)
        {
            if (obj is Topping topping && SelectedMenuItem != null)
            {
                if (SelectedToppings.Contains(topping))
                {
                    SelectedToppings.Remove(topping);
                    topping.IsSelected = false;
                }
                else
                {
                    SelectedToppings.Add(topping);
                    topping.IsSelected = true;
                }
            }
        }

        private void AddToOrder(object? obj)
        {
            if (SelectedMenuItem == null || SelectedTableOrder == null)
                return;

            OrderItem newItem = new()
            {
                MenuItem = SelectedMenuItem,
                Quantity = SelectedQuantity,
                SelectedToppings = new ObservableCollection<Topping>(SelectedToppings)
            };

            // Subscribe to item selection changes
            newItem.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(OrderItem.IsSelected))
                {
                    SelectAllItems = CurrentOrderItems.All(i => i.IsSelected) && CurrentOrderItems.Count > 0;
                }
            };

            SelectedTableOrder.OrderItems.Add(newItem);
            CurrentOrderItems.Add(newItem);

            // Ensure order is in ActiveOrders so it's saved and table status updates
            if (!ActiveOrders.Contains(SelectedTableOrder))
            {
                ActiveOrders.Add(SelectedTableOrder);
            }

            SelectedMenuItem = null;
            SelectedToppings.Clear();
            SelectedQuantity = 1;

            UpdateTableStatus();
            SaveOrders();
            OnPropertyChanged(nameof(CurrentOrderTotal));
        }

        private void RemoveFromOrder(object? obj)
        {
            if (obj is OrderItem item && SelectedTableOrder != null)
            {
                SelectedTableOrder.OrderItems.Remove(item);
                CurrentOrderItems.Remove(item);
                
                // If order is now empty, remove it from active orders
                if (SelectedTableOrder.OrderItems.Count == 0 && ActiveOrders.Contains(SelectedTableOrder))
                {
                    ActiveOrders.Remove(SelectedTableOrder);
                    UpdateTableStatus();
                }
                
                // Reset SelectAllItems if we removed a selected item
                SelectAllItems = CurrentOrderItems.All(i => i.IsSelected) && CurrentOrderItems.Count > 0;
                
                SaveOrders();
                OnPropertyChanged(nameof(CurrentOrderTotal));
            }
        }

        private void ToggleOrderItem(object? obj)
        {
            if (obj is OrderItem item)
            {
                item.IsSelected = !item.IsSelected;
                // Update SelectAllItems state
                SelectAllItems = CurrentOrderItems.All(i => i.IsSelected) && CurrentOrderItems.Count > 0;
            }
        }

        private void SelectAllItems_Executed(object? obj)
        {
            // The actual selection is handled by the SelectAllItems property setter
        }

        private void PrintOrder(object? obj)
        {
            if (SelectedTableOrder == null)
                return;

            // If order not yet in list, add it
            if (!ActiveOrders.Contains(SelectedTableOrder))
            {
                ActiveOrders.Add(SelectedTableOrder);
                UpdateTableStatus();
            }

            string printerName = Settings.DefaultPrinterName ?? "";

            if (string.IsNullOrEmpty(printerName))
            {
                var printers = _printerService.GetAvailablePrinters();
                if (printers.Count == 0)
                {
                    System.Windows.MessageBox.Show("No printers found. Please configure a printer.");
                    return;
                }
                printerName = printers[0];
            }

            var receiptStrings = new Dictionary<string, string>
            {
                { "Header", _localization.GetString("ReceiptHeader") },
                { "Table", _localization.GetString("ReceiptTable") },
                { "Time", _localization.GetString("ReceiptTime") },
                { "Total", _localization.GetString("ReceiptTotal") },
                { "ThankYou", _localization.GetString("ReceiptThankYou") }
            };

            // Get current order number for printing before incrementing
            int currentOrderNumber = Settings.CurrentOrderNumber;

            if (_printerService.PrintTableOrder(SelectedTableOrder, printerName, CurrencySymbol, receiptStrings, true, currentOrderNumber, null, true))
            {
                // Only increment after successful print
                Settings.CurrentOrderNumber++;
                if (Settings.CurrentOrderNumber > 999)
                    Settings.CurrentOrderNumber = 1;

                foreach (var item in SelectedTableOrder.OrderItems)
                {
                    item.IsPrinted = true;
                }
                SaveOrders();
                _dataService.SaveSettings(Settings);
            }
        }

        private void CheckoutTableWithPayment(PaymentMethod paymentMethod)
        {
            if (SelectedTableOrder == null)
                return;

            // Check if any items are selected
            var selectedItems = CurrentOrderItems.Where(i => i.IsSelected).ToList();
            if (selectedItems.Count == 0)
            {
                System.Windows.MessageBox.Show(
                    _localization.GetString("SelectItemsFirst") ?? "Please select items to checkout",
                    _localization.GetString("WarningTitle") ?? "Warning",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning
                );
                return;
            }

            // Calculate total for selected items only
            decimal selectedTotal = selectedItems.Sum(i => i.GetTotal());
            var total = _localization.FormatCurrency(selectedTotal);
            string paymentText = paymentMethod == PaymentMethod.CreditCard
                ? _localization.GetString("PaymentMethodCreditCard")
                : paymentMethod == PaymentMethod.PackageOrder
                ? "Package Order"
                : _localization.GetString("PaymentMethodCash");

            var result = System.Windows.MessageBox.Show(
                $"{_localization.GetString("ConfirmCheckout")}\n{_localization.GetString("TotalLabel")}: {total}\n{_localization.GetString("PaymentMethodLabel")}: {paymentText}",
                _localization.GetString("CheckoutButton"),
                System.Windows.MessageBoxButton.YesNo
            );

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                // Create a transaction record for paid items (for partial checkouts)
                if (selectedItems.Count > 0)
                {
                    var transactionRecord = new TableOrder
                    {
                        TableNumber = SelectedTableOrder.TableNumber,
                        TableLocation = SelectedTableOrder.TableLocation,
                        TableDisplayName = SelectedTableOrder.TableDisplayName,
                        Status = OrderStatus.CheckedOut,
                        CheckedOutAt = DateTime.Now,
                        PaymentMethod = paymentMethod,
                        OrderedAt = SelectedTableOrder.OrderedAt
                    };

                    // Add selected items to the transaction record
                    foreach (var item in selectedItems)
                    {
                        transactionRecord.OrderItems.Add(item);
                    }

                    // Save this transaction to checkouts directory
                    _dataService.SaveCheckout(transactionRecord);
                }

                // Remove only selected items from the order
                foreach (var item in selectedItems.ToList())
                {
                    SelectedTableOrder.OrderItems.Remove(item);
                    CurrentOrderItems.Remove(item);
                }

                // If all items have been paid and removed, clean up the order
                if (SelectedTableOrder.OrderItems.Count == 0)
                {
                    ActiveOrders.Remove(SelectedTableOrder);
                    UpdateTableStatus();
                    SelectedTableOrder = null;
                    CurrentOrderItems.Clear();
                }

                SelectAllItems = false;
                SaveOrders();
                OnPropertyChanged(nameof(CurrentOrderTotal));

                System.Windows.MessageBox.Show(
                    _localization.GetString("CheckoutSuccess") ?? "Checkout successful",
                    _localization.GetString("SuccessTitle") ?? "Success"
                );
            }
        }

        private void PrintReceipt(object? obj)
        {
            if (SelectedTableOrder == null || SelectedTableOrder.OrderItems.Count == 0)
                return;

            string printerName = Settings.DefaultPrinterName ?? "";
            
            if (string.IsNullOrEmpty(printerName))
            {
                var printers = _printerService.GetAvailablePrinters();
                if (printers.Count == 0)
                {
                    System.Windows.MessageBox.Show("No printers found. Please configure a printer.");
                    return;
                }
                printerName = printers[0];
            }

            var receiptStrings = new Dictionary<string, string>
            {
                { "Header", _localization.GetString("ReceiptHeader") },
                { "Table", _localization.GetString("ReceiptTable") },
                { "Time", _localization.GetString("ReceiptTime") },
                { "Total", _localization.GetString("ReceiptTotal") },
                { "ThankYou", _localization.GetString("ReceiptThankYou") }
            };

            _printerService.PrintTableOrder(SelectedTableOrder, printerName, CurrencySymbol, receiptStrings, onlyNewItems: false, 0, RestaurantName, false);
        }

        private void CancelTable(object? obj)
        {
            if (SelectedTableOrder == null)
                return;

            var result = System.Windows.MessageBox.Show(
                _localization.GetString("ConfirmCancel"),
                _localization.GetString("CancelButton"),
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning
            );

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                SelectedTableOrder.Status = OrderStatus.Cancelled;
                SelectedTableOrder.CheckedOutAt = DateTime.Now; // Use CheckedOutAt for cancellation time too
                _dataService.SaveCheckout(SelectedTableOrder);

                ActiveOrders.Remove(SelectedTableOrder);
                UpdateTableStatus();
                SaveOrders();
                SelectedTableOrder = null;
                CurrentOrderItems.Clear();
            }
        }

        private void OpenSettings(object? obj)
        {
            try
            {
                SettingsWindow settings = new();
                settings.DataContext = new SettingsViewModel(_settings, _dataService, this);
                if (settings.ShowDialog() == true)
                {
                    SaveOrders();
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error opening settings: {ex.Message}");
            }
        }

        private void OpenEarnings(object? obj)
        {
            try
            {
                EarningsWindow earnings = new();
                earnings.Owner = System.Windows.Application.Current.MainWindow;
                earnings.DataContext = new EarningsViewModel(_dataService, this);
                earnings.ShowDialog();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error opening earnings: {ex.Message}");
            }
        }

        private void UpdateTableStatus()
        {
            foreach (var table in Tables)
            {
                table.HasActiveOrder = ActiveOrders.Any(o => o.TableNumber == table.Number);
            }
        }

        private void SaveOrders()
        {
            _dataService.SaveOrders(ActiveOrders.ToList());
        }

        public void UpdateLanguage(string language)
        {
            Settings.Language = language;
            _localization.CurrentLanguage = language == "Turkish" 
                ? LocalizationService.Language.Turkish 
                : LocalizationService.Language.English;
            CurrencySymbol = _localization.GetCurrencySymbol();
            
            // Notify all localization properties have changed
            OnPropertyChanged(nameof(LocalizedTitle));
            OnPropertyChanged(nameof(LocalizedSettingsButton));
            OnPropertyChanged(nameof(LocalizedEarningsButton));
            OnPropertyChanged(nameof(LocalizedTablesLabel));
            OnPropertyChanged(nameof(LocalizedMainFoodsTab));
            OnPropertyChanged(nameof(LocalizedDrinksTab));
            OnPropertyChanged(nameof(LocalizedToppingsLabel));
            OnPropertyChanged(nameof(LocalizedSelectedItemLabel));
            OnPropertyChanged(nameof(LocalizedQuantityLabel));
            OnPropertyChanged(nameof(LocalizedAddToOrderButton));
            OnPropertyChanged(nameof(LocalizedCurrentOrderLabel));
            OnPropertyChanged(nameof(LocalizedTotalLabel));
            OnPropertyChanged(nameof(LocalizedPrintButton));
            OnPropertyChanged(nameof(LocalizedCheckoutButton));
            OnPropertyChanged(nameof(LocalizedCancelButton));
            OnPropertyChanged(nameof(LocalizedDeleteButton));
            OnPropertyChanged(nameof(LocalizedTableOrderLabel));
            OnPropertyChanged(nameof(LocalizedQtyLabel));
            OnPropertyChanged(nameof(CurrentOrderTotal));
            OnPropertyChanged(nameof(Localization));

            foreach (var table in Tables)
            {
                table.OnPropertyChanged(nameof(table.HasActiveOrder));
                table.OnPropertyChanged(nameof(table.DisplayName));
            }
            
            _dataService.SaveSettings(Settings);
        }

        public void UpdateRestaurantName(string restaurantName)
        {
            RestaurantName = restaurantName;
            Settings.RestaurantName = restaurantName;
            OnPropertyChanged(nameof(RestaurantName));
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

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
