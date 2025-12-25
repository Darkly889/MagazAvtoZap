using MagazAvtoZap.DataAccess;
using MagazAvtoZap.Models;
using MagazAvtoZap.Views;
using System.Windows;

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

        private void ShowSalesHistory_Click(object sender, RoutedEventArgs e)
        {
            SalesHistoryWindow salesHistoryWindow = new SalesHistoryWindow();
            salesHistoryWindow.Show();
        }

        private void ShowOrderHistory_Click(object sender, RoutedEventArgs e)
        {
            AdminOrderHistoryWindow orderHistoryWindow = new AdminOrderHistoryWindow();
            orderHistoryWindow.Show();
        }
        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
           
            AddEditProductWindow addProductWindow = new AddEditProductWindow();
            if (addProductWindow.ShowDialog() == true)
            {
                LoadProducts();
            }
        }
        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is Product selectedProduct)
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить этот товар?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _databaseService.DeleteProduct(selectedProduct.ProductID);
                    LoadProducts();
                }
            }
            else
            {
                MessageBox.Show("Выберите товар для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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

    }
}
