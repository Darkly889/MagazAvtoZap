using System.Windows;
using System.Windows.Controls;
using MagazAvtoZap.Models;

namespace MagazAvtoZap.Views
{
    public partial class CartWindow : Window
    {
        private Cart _cart;

        public CartWindow(Cart cart)
        {
            InitializeComponent();
            _cart = cart;
            CartItemsDataGrid.ItemsSource = _cart.Items;
            UpdateTotalPrice();
        }

        private void UpdateTotalPrice()
        {
            TotalPriceTextBlock.Text = $"{_cart.GetTotalPrice():C}";
        }

        private void RemoveFromCartButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is CartItem item)
            {
                _cart.RemoveItem(item);
                CartItemsDataGrid.ItemsSource = null;
                CartItemsDataGrid.ItemsSource = _cart.Items;
                UpdateTotalPrice();
            }
        }

        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (_cart.Items.Count == 0)
            {
                MessageBox.Show("Корзина пуста.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CheckoutWindow checkoutWindow = new CheckoutWindow(_cart);
            checkoutWindow.ShowDialog();
            this.Close();
        }
    }
}
