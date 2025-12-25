using System.Linq;
using System.Windows;
using MagazAvtoZap.DataAccess;
using MagazAvtoZap.Models;

namespace MagazAvtoZap.Views
{
    public partial class MainWindow : Window
    {
        private readonly DatabaseService _databaseService;
        private Cart _cart;

        public MainWindow(bool isAdmin = false)
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            _cart = new Cart();
            LoadProducts();
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

        private void ShowCartButton_Click(object sender, RoutedEventArgs e)
        {
            CartWindow cartWindow = new CartWindow(_cart);
            cartWindow.ShowDialog();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите выйти из профиля?", "Подтверждение выхода", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }

        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is Product selectedProduct)
            {
                if (selectedProduct.AvailableQuantity <= 0)
                {
                    MessageBox.Show("Этот товар недоступен для заказа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity <= 0)
                {
                    MessageBox.Show("Количество должно быть положительным числом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (quantity > selectedProduct.AvailableQuantity)
                {
                    MessageBox.Show($"Недостаточно товара на складе. Доступно: {selectedProduct.AvailableQuantity}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                CartItem existingItem = _cart.Items.FirstOrDefault(item => item.ProductId == selectedProduct.ProductID);

                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                }
                else
                {
                    CartItem cartItem = new CartItem
                    {
                        ProductId = selectedProduct.ProductID,
                        Name = selectedProduct.Name,
                        Price = selectedProduct.Price,
                        Quantity = quantity
                    };
                    _cart.AddItem(cartItem);
                }

                MessageBox.Show("Товар добавлен в корзину.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Выберите товар для добавления в корзину.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
