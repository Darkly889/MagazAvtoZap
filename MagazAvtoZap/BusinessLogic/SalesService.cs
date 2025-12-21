using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using System.Windows;
using MagazAvtoZap.DataAccess;
using MagazAvtoZap.Models;

namespace MagazAvtoZap.BusinessLogic
{
    public class SalesService
    {
        private readonly DatabaseService _databaseService;
        private readonly ValidationService _validationService;

        public SalesService()
        {
            _databaseService = new DatabaseService();
            _validationService = new ValidationService();
        }

        public bool ProcessSale(Sale sale)
        {
            if (!_validationService.ValidateObject(sale, out List<ValidationResult> validationResults))
            {
                string errorMessage = string.Join("\n", validationResults.Select(vr => vr.ErrorMessage));
                MessageBox.Show($"Ошибка валидации:\n{errorMessage}", "ОШИБКА", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            var inventoryService = new InventoryService();

            if (!inventoryService.CheckStock(sale.ProductId, sale.Quantity))
            {
                MessageBox.Show("Недостаточно товара на складе.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            using (SqlConnection connection = new SqlConnection(_databaseService.ConnectionString))
            {
                connection.Open();
                SqlTransaction transaction = null;

                try
                {
                    transaction = connection.BeginTransaction();

                    SqlCommand getStockCommand = new SqlCommand(
                        "SELECT StockQuantity FROM Products WHERE ProductID = @ProductID",
                        connection,
                        transaction);
                    getStockCommand.Parameters.AddWithValue("@ProductID", sale.ProductId);
                    object stockResult = getStockCommand.ExecuteScalar();

                    if (stockResult == null || stockResult == DBNull.Value)
                    {
                        MessageBox.Show("Ошибка получения остатка товара.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }

                    int currentStock = Convert.ToInt32(stockResult);

                    if (currentStock < sale.Quantity)
                    {
                        MessageBox.Show("Недостаточно товара на складе.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }

                    SqlCommand updateStockCommand = new SqlCommand(
                        "UPDATE Products SET StockQuantity = StockQuantity - @Quantity WHERE ProductID = @ProductID",
                        connection,
                        transaction);
                    updateStockCommand.Parameters.AddWithValue("@Quantity", sale.Quantity);
                    updateStockCommand.Parameters.AddWithValue("@ProductID", sale.ProductId);
                    updateStockCommand.ExecuteNonQuery();

                    SqlCommand addSaleCommand = new SqlCommand(
                        "INSERT INTO Sales (SaleDate, ProductID, Quantity, TotalPrice) VALUES (@SaleDate, @ProductID, @Quantity, @TotalPrice)",
                        connection,
                        transaction);
                    addSaleCommand.Parameters.AddWithValue("@SaleDate", sale.SaleDate);
                    addSaleCommand.Parameters.AddWithValue("@ProductID", sale.ProductId);
                    addSaleCommand.Parameters.AddWithValue("@Quantity", sale.Quantity);
                    addSaleCommand.Parameters.AddWithValue("@TotalPrice", sale.TotalPrice);
                    addSaleCommand.ExecuteNonQuery();

                    transaction.Commit();
                    MessageBox.Show("Продажа успешно оформлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
                catch (SqlException ex)
                {
                    if (transaction != null)
                        transaction.Rollback();
                    MessageBox.Show($"Ошибка при оформлении продажи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }

        public bool ProcessCartCheckout(Cart cart)
        {
            using (SqlConnection connection = new SqlConnection(_databaseService.ConnectionString))
            {
                connection.Open();
                SqlTransaction transaction = null;

                try
                {
                    transaction = connection.BeginTransaction();

                    foreach (var item in cart.Items)
                    {
                        SqlCommand getStockCommand = new SqlCommand(
                            "SELECT StockQuantity FROM Products WHERE ProductID = @ProductID",
                            connection,
                            transaction);
                        getStockCommand.Parameters.AddWithValue("@ProductID", item.ProductId);
                        object stockResult = getStockCommand.ExecuteScalar();

                        if (stockResult == null || stockResult == DBNull.Value)
                        {
                            MessageBox.Show($"Ошибка получения остатка товара {item.Name}.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return false;
                        }

                        int currentStock = Convert.ToInt32(stockResult);

                        if (currentStock < item.Quantity)
                        {
                            MessageBox.Show($"Недостаточно товара {item.Name} на складе.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return false;
                        }

                        SqlCommand updateStockCommand = new SqlCommand(
                            "UPDATE Products SET StockQuantity = StockQuantity - @Quantity WHERE ProductID = @ProductID",
                            connection,
                            transaction);
                        updateStockCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                        updateStockCommand.Parameters.AddWithValue("@ProductID", item.ProductId);
                        updateStockCommand.ExecuteNonQuery();

                        SqlCommand addSaleCommand = new SqlCommand(
                            "INSERT INTO Sales (SaleDate, ProductID, Quantity, TotalPrice) VALUES (@SaleDate, @ProductID, @Quantity, @TotalPrice)",
                            connection,
                            transaction);
                        addSaleCommand.Parameters.AddWithValue("@SaleDate", DateTime.Now);
                        addSaleCommand.Parameters.AddWithValue("@ProductID", item.ProductId);
                        addSaleCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                        addSaleCommand.Parameters.AddWithValue("@TotalPrice", item.Price * item.Quantity);
                        addSaleCommand.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    MessageBox.Show("Все товары успешно проданы!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
                catch (SqlException ex)
                {
                    if (transaction != null)
                        transaction.Rollback();
                    MessageBox.Show($"Ошибка при оформлении продажи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }
    }
}
