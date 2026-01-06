using System.Windows;
using RestaurantApp.ViewModels;

namespace RestaurantApp
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is SettingsViewModel vm)
            {
                vm.SaveCommand.Execute(null);
                DialogResult = true;
                Close();
            }
        }
    }
}
