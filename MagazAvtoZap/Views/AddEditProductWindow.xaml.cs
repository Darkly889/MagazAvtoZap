using MagazAvtoZap.DataAccess;
using MagazAvtoZap.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Windows;

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
            _product.Name = NameTextBox.Text;
            _product.Supplier = SupplierTextBox.Text;
            _product.Description = DescriptionTextBox.Text;

            if (CategoryComboBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите категорию.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _product.CategoryID = (int)CategoryComboBox.SelectedValue;

            if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Цена должна быть положительным числом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _product.Price = price;

            if (!int.TryParse(StockQuantityTextBox.Text, out int stockQuantity) || stockQuantity < 0)
            {
                MessageBox.Show("Количество на складе должно быть неотрицательным целым числом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _product.StockQuantity = stockQuantity;

            // Проверка валидации с использованием аннотаций данных
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(_product);

            if (!Validator.TryValidateObject(_product, validationContext, validationResults, true))
            {
                string errorMessage = string.Join(Environment.NewLine, validationResults.Select(r => r.ErrorMessage));
                MessageBox.Show($"Ошибка валидации:\n{errorMessage}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                if (_product.ProductID == 0)
                {
                    _databaseService.AddProduct(_product);
                }
                else
                {
                    _databaseService.UpdateProduct(_product);
                }
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения товара: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
