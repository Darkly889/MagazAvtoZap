using MagazAvtoZap.DataAccess;
using MagazAvtoZap.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MagazAvtoZap.Views
{
    public partial class CheckoutWindow : Window
    {
        private Cart _cart;
        private DatabaseService _databaseService;

        public CheckoutWindow(Cart cart)
        {
            InitializeComponent();
            _cart = cart;
            _databaseService = new DatabaseService();
        }

        private void DeliveryCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AddressLabel.Visibility = Visibility.Visible;
            AddressTextBox.Visibility = Visibility.Visible;
        }

        private void DeliveryCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AddressLabel.Visibility = Visibility.Collapsed;
            AddressTextBox.Visibility = Visibility.Collapsed;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            string fullName = FullNameTextBox.Text;
            string phone = PhoneTextBox.Text;
            string address = DeliveryCheckBox.IsChecked == true ? AddressTextBox.Text : "Самовывоз";
            string paymentMethod = (PaymentMethodComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Пожалуйста, заполните обязательные поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string orderNumber = GenerateOrderNumber();
            SaveOrder(fullName, phone, address, paymentMethod, orderNumber);

            MessageBox.Show($"Заказ успешно оформлен! Номер вашего заказа: {orderNumber}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.Now:yyyyMMddHHmmss}";
        }

        private void SaveOrder(string fullName, string phone, string address, string paymentMethod, string orderNumber)
        {
            var order = new Order
            {
                OrderNumber = orderNumber,
                FullName = fullName,
                Phone = phone,
                Address = address,
                PaymentMethod = paymentMethod,
                OrderDate = DateTime.Now,
                Status = "Новый",
                Items = _cart.Items
            };

            _databaseService.AddOrder(order);
        }
    }
}
