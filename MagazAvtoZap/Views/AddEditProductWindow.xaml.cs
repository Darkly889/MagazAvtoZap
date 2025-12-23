using System;
using System.Windows;
using MagazAvtoZap.DataAccess;
using MagazAvtoZap.Models;

namespace MagazAvtoZap.Views
{
    public partial class AddEditProductWindow : Window
    {
        private readonly DatabaseService _databaseService;
        private Product _product;

        public AddEditProductWindow()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            Title = "Добавление товара";
            LoadCategories();
        }

        public AddEditProductWindow(Product product)
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            Title = "Редактирование товара";
            _product = product;
            LoadCategories();
            LoadProductData();
        }

        private void LoadCategories()
        {
            var categories = _databaseService.GetCategories();
            CategoryComboBox.ItemsSource = categories;
            CategoryComboBox.DisplayMemberPath = "Name";
            CategoryComboBox.SelectedValuePath = "CategoryID";
        }

        private void LoadProductData()
        {
            NameTextBox.Text = _product.Name;
            CategoryComboBox.SelectedValue = _product.CategoryID;
            SupplierTextBox.Text = _product.Supplier;
            PriceTextBox.Text = _product.Price.ToString();
            StockQuantityTextBox.Text = _product.StockQuantity.ToString();
            DescriptionTextBox.Text = _product.Description;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NameTextBox.Text) || string.IsNullOrEmpty(PriceTextBox.Text) || string.IsNullOrEmpty(StockQuantityTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price < 0.01m)
            {
                MessageBox.Show("Цена должна быть не менее 0.01.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(StockQuantityTextBox.Text, out int stockQuantity) || stockQuantity < 1)
            {
                MessageBox.Show("Остаток на складе должен быть не менее 1.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var product = new Product
            {
                Name = NameTextBox.Text,
                CategoryID = (int)CategoryComboBox.SelectedValue,
                Supplier = SupplierTextBox.Text,
                Price = price,
                StockQuantity = stockQuantity,
                Description = DescriptionTextBox.Text
            };

            if (_product != null)
            {
                product.ProductID = _product.ProductID;
                _databaseService.UpdateProduct(product);
            }
            else
            {
                _databaseService.AddProduct(product);
            }

            this.DialogResult = true;
            this.Close();
        }
    }
}
