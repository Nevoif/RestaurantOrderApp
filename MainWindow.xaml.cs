using System.Windows;
using RestaurantApp.ViewModels;

namespace RestaurantApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void DecrementQuantity(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm && vm.SelectedQuantity > 1)
                vm.SelectedQuantity--;
        }

        private void IncrementQuantity(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
                vm.SelectedQuantity++;
        }
    }
}
