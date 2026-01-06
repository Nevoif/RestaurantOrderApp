using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using RestaurantApp.Models;
using RestaurantApp.Services;

namespace RestaurantApp.ViewModels
{
    public class EarningsViewModel : INotifyPropertyChanged
    {
        private readonly DataService _dataService;
        private readonly MainViewModel _mainViewModel;
        private string _selectedMonth;
        private ObservableCollection<TableOrder> _checkouts = new();
        private decimal _totalEarnings;

        public ObservableCollection<string> AvailableMonths { get; } = new();

        public string SelectedMonth
        {
            get => _selectedMonth;
            set
            {
                if (SetProperty(ref _selectedMonth, value))
                {
                    LoadCheckouts();
                }
            }
        }

        public ObservableCollection<TableOrder> Checkouts
        {
            get => _checkouts;
            set => SetProperty(ref _checkouts, value);
        }

        public decimal TotalEarnings
        {
            get => _totalEarnings;
            set => SetProperty(ref _totalEarnings, value);
        }

        public string FormattedTotalEarnings => _mainViewModel.Localization.FormatCurrency(TotalEarnings);

        public string LocalizedEarningsTitle => _mainViewModel.Localization.GetString("EarningsButton");
        public string LocalizedMonthLabel => _mainViewModel.Localization.GetString("MonthLabel");
        public string LocalizedTotalLabel => _mainViewModel.Localization.GetString("TotalLabel");
        public string LocalizedTableLabel => _mainViewModel.Localization.GetString("Table");
        public string LocalizedTimeLabel => _mainViewModel.Localization.GetString("Time");
        public string LocalizedStatusLabel => _mainViewModel.Localization.GetString("Status");
        public string LocalizedItemsLabel => _mainViewModel.Localization.GetString("ItemsLabel");
        public string LocalizedCloseButton => _mainViewModel.Localization.GetString("CloseButton");

        public EarningsViewModel(DataService dataService, MainViewModel mainViewModel)
        {
            _dataService = dataService;
            _mainViewModel = mainViewModel;

            var months = _dataService.GetAvailableMonths();
            foreach (var month in months)
            {
                AvailableMonths.Add(month);
            }

            if (AvailableMonths.Count > 0)
            {
                SelectedMonth = AvailableMonths[0];
            }
            else
            {
                // if no checkouts yet, add current month
                string currentMonth = DateTime.Now.ToString("yyyy-MM");
                AvailableMonths.Add(currentMonth);
                SelectedMonth = currentMonth;
            }
        }

        private void LoadCheckouts()
        {
            if (string.IsNullOrEmpty(SelectedMonth)) return;

            var parts = SelectedMonth.Split('-');
            if (parts.Length == 2 && int.TryParse(parts[0], out int year) && int.TryParse(parts[1], out int month))
            {
                var loaded = _dataService.LoadMonthlyCheckouts(year, month);
                Checkouts.Clear();
                decimal total = 0;
                foreach (var checkout in loaded.OrderByDescending(o => o.CheckedOutAt))
                {
                    Checkouts.Add(checkout);
                    if (checkout.Status == OrderStatus.CheckedOut)
                    {
                        total += checkout.GetTotal();
                    }
                }
                TotalEarnings = total;
                OnPropertyChanged(nameof(FormattedTotalEarnings));
            }
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
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
