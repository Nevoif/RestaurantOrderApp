using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RestaurantApp.Models
{
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
                return false;

            field = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    public class Topping : ObservableObject
    {
        private int _id;
        private string _name = string.Empty;
        private decimal _price;
        private bool _isSelected;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        public decimal Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }

    public class MenuItem : ObservableObject
    {
        private int _id;
        private string _name = string.Empty;
        private string _category = string.Empty;
        private decimal _price;
        private ObservableCollection<Topping> _availableToppings = new();

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        public string Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }
        public decimal Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }
        public ObservableCollection<Topping> AvailableToppings
        {
            get => _availableToppings;
            set => SetProperty(ref _availableToppings, value);
        }
    }

    public class OrderItem : ObservableObject
    {
        private int _id;
        private MenuItem _menuItem = null!;
        private ObservableCollection<Topping> _selectedToppings = new();
        private int _quantity = 1;
        private bool _isPrinted;
        private bool _isSelected = false;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }
        public MenuItem MenuItem
        {
            get => _menuItem;
            set
            {
                if (SetProperty(ref _menuItem, value))
                    OnPropertyChanged(nameof(Total));
            }
        }
        public ObservableCollection<Topping> SelectedToppings
        {
            get => _selectedToppings;
            set
            {
                if (SetProperty(ref _selectedToppings, value))
                    OnPropertyChanged(nameof(Total));
            }
        }
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (SetProperty(ref _quantity, value))
                    OnPropertyChanged(nameof(Total));
            }
        }

        public bool IsPrinted
        {
            get => _isPrinted;
            set => SetProperty(ref _isPrinted, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public decimal Total => GetTotal();

        public decimal GetTotal()
        {
            decimal toppingTotal = SelectedToppings.Sum(t => t.Price);
            return (MenuItem.Price + toppingTotal) * Quantity;
        }
    }

    public class TableOrder : ObservableObject
    {
        private int _tableNumber;
        private string _tableLocation = "inside";
        private string _tableDisplayName = string.Empty;
        private ObservableCollection<OrderItem> _orderItems = new();
        private DateTime _orderedAt = DateTime.Now;
        private DateTime? _checkedOutAt;
        private OrderStatus _status = OrderStatus.Active;
        private PaymentMethod _paymentMethod = PaymentMethod.None;

        public int TableNumber
        {
            get => _tableNumber;
            set => SetProperty(ref _tableNumber, value);
        }
        public string TableLocation
        {
            get => _tableLocation;
            set => SetProperty(ref _tableLocation, value);
        }
        public string TableDisplayName
        {
            get => _tableDisplayName;
            set => SetProperty(ref _tableDisplayName, value);
        }
        public ObservableCollection<OrderItem> OrderItems
        {
            get => _orderItems;
            set => SetProperty(ref _orderItems, value);
        }
        public DateTime OrderedAt
        {
            get => _orderedAt;
            set => SetProperty(ref _orderedAt, value);
        }

        public DateTime? CheckedOutAt
        {
            get => _checkedOutAt;
            set => SetProperty(ref _checkedOutAt, value);
        }
        public OrderStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public PaymentMethod PaymentMethod
        {
            get => _paymentMethod;
            set => SetProperty(ref _paymentMethod, value);
        }

        public decimal Total => GetTotal();

        public decimal GetTotal()
        {
            return OrderItems.Sum(item => item.GetTotal());
        }
    }

    public class Table : ObservableObject
    {
        private int _number;
        private string _location = "inside";
        private string _displayName = string.Empty;
        private bool _hasActiveOrder;
        private bool _isSelected;

        public int Number
        {
            get => _number;
            set => SetProperty(ref _number, value);
        }
        public string Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }
        public string DisplayName
        {
            get => _displayName;
            set => SetProperty(ref _displayName, value);
        }
        public bool HasActiveOrder
        {
            get => _hasActiveOrder;
            set => SetProperty(ref _hasActiveOrder, value);
        }
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }

    public enum OrderStatus
    {
        Active,
        CheckedOut,
        Cancelled
    }

    public enum PaymentMethod
    {
        None,
        CreditCard,
        Cash,
        PackageOrder
    }

    public class AppSettings
    {
        public int TotalTables { get; set; } = 10;
        public int InsideTables { get; set; } = 6;
        public int OutsideTables { get; set; } = 4;
        public string DefaultPrinterName { get; set; } = string.Empty;
        public string NetworkPrinterIP { get; set; } = "192.168.1.100";
        public int NetworkPrinterPort { get; set; } = 9100;
        public List<MenuItem> MenuItems { get; set; } = new();
        public List<Table> Tables { get; set; } = new();
        public string Language { get; set; } = "English"; // language change
        public string RestaurantName { get; set; } = "Restaurant"; // restaurant name
        public int CurrentOrderNumber { get; set; } = 0; // order number for printing
    }
}
