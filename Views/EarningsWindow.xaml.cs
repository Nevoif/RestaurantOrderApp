using System.Windows;

namespace RestaurantApp.Views
{
    public partial class EarningsWindow : Window
    {
        public EarningsWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
