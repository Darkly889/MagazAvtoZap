using System;
using System.Windows;
using System.Windows.Controls;
using MagazAvtoZap.BusinessLogic;
using MagazAvtoZap.DataAccess;
using MagazAvtoZap.Models;

namespace MagazAvtoZap.Views
{
    public partial class MainWindow : Window
    {
        private readonly DatabaseService _databaseService;
        private readonly SalesService _salesService;
        private Cart _cart = new Cart();
        private bool _isAdmin;

        public MainWindow()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            _salesService = new SalesService();
            _isAdmin = false;
            LoadProducts();
        }

        public MainWindow(bool isAdmin)
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            _salesService = new SalesService();
            _isAdmin = isAdmin;
            LoadProducts();

            if (_isAdmin)
            {
                AdminButton.Visibility = Visibility.Visible;
            }
        }

        private void LoadProducts()
        {
            var products = _databaseService.GetProducts();
            ProductsDataGrid.ItemsSource = products;
        }

        private void RefreshProductsButton_Click(object sender, RoutedEventArgs e)
        {
            LoadProducts();
        }

        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            string productName = ProductNameTextBox.Text;
            if (string.IsNullOrEmpty(productName))
            {
                MessageBox.Show("Введите название товара.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество товара.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var cartItem = _databaseService.GetProductForCart(productName);

            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                _cart.AddItem(cartItem);
                CartItemsDataGrid.ItemsSource = null;
                CartItemsDataGrid.ItemsSource = _cart.Items;
                TotalPriceTextBlock.Text = $"Итого: {_cart.GetTotalPrice():C}";
            }
            else
            {
                MessageBox.Show("Товар не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RemoveFromCartButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is CartItem item)
            {
                _cart.RemoveItem(item);
                CartItemsDataGrid.ItemsSource = null;
                CartItemsDataGrid.ItemsSource = _cart.Items;
                TotalPriceTextBlock.Text = $"Итого: {_cart.GetTotalPrice():C}";
            }
        }

        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (_cart.Items.Count == 0)
            {
                MessageBox.Show("Корзина пуста.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_salesService.ProcessCartCheckout(_cart))
            {
                _cart.Clear();
                CartItemsDataGrid.ItemsSource = null;
                TotalPriceTextBlock.Text = "Итого: 0 руб.";
                LoadProducts();
            }
        }

        private void ShowSalesHistory_Click(object sender, RoutedEventArgs e)
        {
            SalesHistoryWindow salesHistoryWindow = new SalesHistoryWindow();
            salesHistoryWindow.Show();
        }

        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow adminWindow = new AdminWindow();
            adminWindow.Show();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
