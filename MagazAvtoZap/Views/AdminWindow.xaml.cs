using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MagazAvtoZap.DataAccess;
using MagazAvtoZap.Models;

namespace MagazAvtoZap.Views
{
    public partial class AdminWindow : Window
    {
        private readonly DatabaseService _databaseService;
        private List<Product> _products = new List<Product>();
        private List<Category> _categories = new List<Category>();

        public AdminWindow()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            LoadProducts();
            LoadCategories();
        }

        private void LoadProducts()
        {
            _products = _databaseService.GetProducts();
            ProductsDataGrid.ItemsSource = _products;
        }

        private void LoadCategories()
        {
            _categories = _databaseService.GetCategories();
            CategoryComboBox.ItemsSource = _categories;
            CategoryComboBox.DisplayMemberPath = "Name";
            CategoryComboBox.SelectedValuePath = "CategoryID";
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NameTextBox.Text) ||
                CategoryComboBox.SelectedItem == null ||
                !decimal.TryParse(PriceTextBox.Text, out decimal price) ||
                !int.TryParse(StockQuantityTextBox.Text, out int stockQuantity))
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Product product = new Product
            {
                Name = NameTextBox.Text,
                CategoryID = (int)CategoryComboBox.SelectedValue,
                Supplier = SupplierTextBox.Text,
                Price = price,
                StockQuantity = stockQuantity,
                Description = DescriptionTextBox.Text
            };

            _databaseService.AddProduct(product);
            LoadProducts();
            ClearForm();
        }

        private void UpdateProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is not Product selectedProduct)
            {
                MessageBox.Show("Выберите товар для обновления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(NameTextBox.Text) ||
                CategoryComboBox.SelectedItem == null ||
                !decimal.TryParse(PriceTextBox.Text, out decimal price) ||
                !int.TryParse(StockQuantityTextBox.Text, out int stockQuantity))
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Product product = new Product
            {
                ProductID = selectedProduct.ProductID,
                Name = NameTextBox.Text,
                CategoryID = (int)CategoryComboBox.SelectedValue,
                Supplier = SupplierTextBox.Text,
                Price = price,
                StockQuantity = stockQuantity,
                Description = DescriptionTextBox.Text
            };

            _databaseService.UpdateProduct(product);
            LoadProducts();
            ClearForm();
        }

        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int productId)
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить этот товар?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _databaseService.DeleteProduct(productId);
                    LoadProducts();
                }
            }
        }

        private void ClearForm()
        {
            NameTextBox.Clear();
            CategoryComboBox.SelectedIndex = -1;
            SupplierTextBox.Clear();
            PriceTextBox.Clear();
            StockQuantityTextBox.Clear();
            DescriptionTextBox.Clear();
        }

        private void ProductsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is Product selectedProduct)
            {
                NameTextBox.Text = selectedProduct.Name;
                CategoryComboBox.SelectedValue = selectedProduct.CategoryID;
                SupplierTextBox.Text = selectedProduct.Supplier;
                PriceTextBox.Text = selectedProduct.Price.ToString();
                StockQuantityTextBox.Text = selectedProduct.StockQuantity.ToString();
                DescriptionTextBox.Text = selectedProduct.Description ?? string.Empty;
            }
        }
    }
}
