using System.Windows;
using System.Windows.Controls;
using MagazAvtoZap.DataAccess;
using MagazAvtoZap.Models;

namespace MagazAvtoZap.Views
{
    public partial class AdminWindow : Window
    {
        private readonly DatabaseService _databaseService;

        public AdminWindow()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            LoadProducts();
        }

        private void LoadProducts()
        {
            var products = _databaseService.GetProducts();
            ProductsDataGrid.ItemsSource = products;
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            AddEditProductWindow addProductWindow = new AddEditProductWindow();
            if (addProductWindow.ShowDialog() == true)
            {
                LoadProducts();
            }
        }

        private void EditProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is Product selectedProduct)
            {
                AddEditProductWindow editProductWindow = new AddEditProductWindow(selectedProduct);
                if (editProductWindow.ShowDialog() == true)
                {
                    LoadProducts();
                }
            }
            else
            {
                MessageBox.Show("Выберите товар для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowOrderHistory_Click(object sender, RoutedEventArgs e)
        {
            AdminOrderHistoryWindow orderHistoryWindow = new AdminOrderHistoryWindow();
            orderHistoryWindow.Show();
        }
    }
}
