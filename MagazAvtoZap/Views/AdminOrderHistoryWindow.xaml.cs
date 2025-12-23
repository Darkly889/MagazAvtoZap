using System.Windows;
using System.Windows.Controls;
using MagazAvtoZap.DataAccess;

namespace MagazAvtoZap.Views
{
    public partial class AdminOrderHistoryWindow : Window
    {
        private readonly DatabaseService _databaseService;

        public AdminOrderHistoryWindow()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            LoadOrderHistory();
        }

        private void LoadOrderHistory()
        {
            var orderHistory = _databaseService.GetOrderHistory();
            OrderHistoryDataGrid.ItemsSource = orderHistory;
        }

        private void ViewOrderDetails_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string orderNumber)
            {
                AdminOrderDetailsWindow orderDetailsWindow = new AdminOrderDetailsWindow(orderNumber);
                orderDetailsWindow.ShowDialog();
            }
        }
    }
}
