using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            _cart.PropertyChanged += Cart_PropertyChanged;
            AdminButton.Visibility = Visibility.Collapsed; 
        }

        public MainWindow(bool isAdmin)
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            _salesService = new SalesService();
            _isAdmin = isAdmin;
            LoadProducts();
            _cart.PropertyChanged += Cart_PropertyChanged;

            if (_isAdmin)
            {
                AdminButton.Visibility = Visibility.Visible; 
            }
            else
            {
                AdminButton.Visibility = Visibility.Collapsed; 
            }
        }

        private void Cart_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateCartItemCount();
        }

        private void UpdateCartItemCount()
        {
            int itemCount = _cart.Items.Sum(item => item.Quantity);
            CartItemCountTextBlock.Text = itemCount.ToString();
        }

        private void LoadProducts()
        {
            try
            {
                var products = _databaseService.GetProducts();
                ProductsDataGrid.ItemsSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки товаров: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                MessageBox.Show("Товар добавлен в корзину.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Товар не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CartImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CartWindow cartWindow = new CartWindow(_cart);
            cartWindow.ShowDialog();
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
