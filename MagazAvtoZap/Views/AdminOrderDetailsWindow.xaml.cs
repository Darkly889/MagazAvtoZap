using System.Windows;
using MagazAvtoZap.DataAccess;
using MagazAvtoZap.Models;

namespace MagazAvtoZap.Views
{
    public partial class AdminOrderDetailsWindow : Window
    {
        private readonly DatabaseService _databaseService;
        private string _orderNumber;

        public AdminOrderDetailsWindow(string orderNumber)
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            _orderNumber = orderNumber;
            LoadOrderDetails();
        }

        private void LoadOrderDetails()
        {
            var order = _databaseService.GetOrderDetails(_orderNumber);

            if (order != null)
            {
                OrderNumberTextBlock.Text = $"Заказ №{order.OrderNumber}";
                OrderDateTextBlock.Text = $"Дата заказа: {order.OrderDate:dd.MM.yyyy HH:mm}";
                StatusTextBlock.Text = $"Статус: {order.Status}";
                FullNameTextBlock.Text = $"ФИО: {order.FullName}";
                PhoneTextBlock.Text = $"Телефон: {order.Phone}";
                AddressTextBlock.Text = $"Адрес: {order.Address}";
                PaymentMethodTextBlock.Text = $"Способ оплаты: {order.PaymentMethod}";

                OrderItemsDataGrid.ItemsSource = order.Items;
            }
        }

        private void ChangeStatusButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeOrderStatusWindow changeStatusWindow = new ChangeOrderStatusWindow(_orderNumber);
            if (changeStatusWindow.ShowDialog() == true)
            {
                LoadOrderDetails();
            }
        }

    }
}
