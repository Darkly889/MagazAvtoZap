using System;
using System.Windows;
using System.Windows.Controls;
using MagazAvtoZap.Models;
using MagazAvtoZap.DataAccess;

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

            DeliveryMethodComboBox.SelectionChanged += DeliveryMethodComboBox_SelectionChanged;
        }

        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.Now:yyyyMMddHHmmss}";
        }

        private void DeliveryMethodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DeliveryMethodComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                if (selectedItem.Content.ToString() == "Доставка")
                {
                    AddressLabel.Visibility = Visibility.Visible;
                    AddressTextBox.Visibility = Visibility.Visible;
                }
                else
                {
                    AddressLabel.Visibility = Visibility.Collapsed;
                    AddressTextBox.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FullNameTextBox.Text) || string.IsNullOrEmpty(PhoneTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string deliveryMethod = (DeliveryMethodComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (string.IsNullOrEmpty(deliveryMethod))
            {
                MessageBox.Show("Выберите способ получения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string address = deliveryMethod == "Доставка" ? AddressTextBox.Text : "Самовывоз";

            string paymentMethod = (PaymentMethodComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (string.IsNullOrEmpty(paymentMethod))
            {
                MessageBox.Show("Выберите способ оплаты.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            
            string orderNumber = GenerateOrderNumber();

            
            Order order = new Order
            {
                OrderNumber = orderNumber,
                FullName = FullNameTextBox.Text,
                Phone = PhoneTextBox.Text,
                Address = address,
                PaymentMethod = paymentMethod,
                OrderDate = DateTime.Now,
                Status = "Новый",
                Items = new System.Collections.Generic.List<CartItem>(_cart.Items)
            };

            
            _databaseService.AddOrder(order);

            
            MessageBox.Show($"Заказ успешно оформлен! Номер вашего заказа: {orderNumber}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            this.DialogResult = true;
            this.Close();
        }
    }
}
