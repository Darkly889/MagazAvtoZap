using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Windows;
using MagazAvtoZap.Models;
using System.Configuration;

namespace MagazAvtoZap.DataAccess
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["MagazAvtoZapConnection"]?.ConnectionString
                                ?? throw new InvalidOperationException("Connection string not found.");
        }

        public string ConnectionString => _connectionString;

        public List<Product> GetProducts()
        {
            var products = new List<Product>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(
                        @"SELECT
                            p.ProductID, p.Name, p.CategoryID, c.Name AS CategoryName,
                            p.Supplier, p.Price, p.StockQuantity, p.Description
                          FROM
                            Products p
                          JOIN
                            Categories c ON p.CategoryID = c.CategoryID", connection);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new Product
                            {
                                ProductID = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                CategoryID = reader.GetInt32(2),
                                CategoryName = reader.GetString(3),
                                Supplier = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Price = reader.GetDecimal(5),
                                StockQuantity = reader.GetInt32(6),
                                Description = reader.IsDBNull(7) ? null : reader.GetString(7)
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка загрузки товаров: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return products;
        }

        public int GetProductStock(int productId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT StockQuantity FROM Products WHERE ProductID = @ProductID", connection);
                    command.Parameters.AddWithValue("@ProductID", productId);
                    object result = command.ExecuteScalar();
                    return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
        }

        public int GetProductIdByName(string productName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT ProductID FROM Products WHERE Name = @Name", connection);
                    command.Parameters.AddWithValue("@Name", productName);
                    object result = command.ExecuteScalar();
                    return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
        }

        public void UpdateProductStock(int productId, int soldQuantity)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(
                        "UPDATE Products SET StockQuantity = StockQuantity - @Quantity WHERE ProductID = @ProductID",
                        connection);
                    command.Parameters.AddWithValue("@Quantity", soldQuantity);
                    command.Parameters.AddWithValue("@ProductID", productId);
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка обновления остатков: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void AddSale(Sale sale)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(
                        "INSERT INTO Sales (SaleDate, ProductID, Quantity, TotalPrice) VALUES (@SaleDate, @ProductID, @Quantity, @TotalPrice)",
                        connection);
                    command.Parameters.AddWithValue("@SaleDate", sale.SaleDate);
                    command.Parameters.AddWithValue("@ProductID", sale.ProductId);
                    command.Parameters.AddWithValue("@Quantity", sale.Quantity);
                    command.Parameters.AddWithValue("@TotalPrice", sale.TotalPrice);
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка добавления продажи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public CartItem? GetProductForCart(string productName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(
                        @"SELECT ProductID, Name, Price FROM Products WHERE Name = @Name", connection);
                    command.Parameters.AddWithValue("@Name", productName);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new CartItem
                            {
                                ProductId = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Price = reader.GetDecimal(2),
                                Quantity = 1
                            };
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка получения товара: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return null;
        }

        public List<SalesHistory> GetSalesHistory()
        {
            var salesHistory = new List<SalesHistory>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(
                        @"SELECT s.SaleID, s.SaleDate, s.ProductID, s.Quantity, s.TotalPrice, p.Name AS ProductName
                          FROM Sales s
                          JOIN Products p ON s.ProductID = p.ProductID
                          ORDER BY s.SaleDate DESC", connection);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            salesHistory.Add(new SalesHistory
                            {
                                SaleID = reader.GetInt32(0),
                                SaleDate = reader.GetDateTime(1),
                                ProductID = reader.GetInt32(2),
                                Quantity = reader.GetInt32(3),
                                TotalPrice = reader.GetDecimal(4),
                                ProductName = reader.GetString(5)
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка загрузки истории продаж: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return salesHistory;
        }

        public List<Category> GetCategories()
        {
            var categories = new List<Category>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT CategoryID, Name FROM Categories", connection);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(new Category
                            {
                                CategoryID = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка загрузки категорий: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return categories;
        }

        public void AddProduct(Product product)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(
                        "INSERT INTO Products (Name, CategoryID, Supplier, Price, StockQuantity, Description) VALUES (@Name, @CategoryID, @Supplier, @Price, @StockQuantity, @Description)",
                        connection);
                    command.Parameters.AddWithValue("@Name", product.Name);
                    command.Parameters.AddWithValue("@CategoryID", product.CategoryID);
                    command.Parameters.AddWithValue("@Supplier", product.Supplier ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Price", product.Price);
                    command.Parameters.AddWithValue("@StockQuantity", product.StockQuantity);
                    command.Parameters.AddWithValue("@Description", product.Description ?? (object)DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка добавления товара: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateProduct(Product product)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(
                        "UPDATE Products SET Name = @Name, CategoryID = @CategoryID, Supplier = @Supplier, Price = @Price, StockQuantity = @StockQuantity, Description = @Description WHERE ProductID = @ProductID",
                        connection);
                    command.Parameters.AddWithValue("@ProductID", product.ProductID);
                    command.Parameters.AddWithValue("@Name", product.Name);
                    command.Parameters.AddWithValue("@CategoryID", product.CategoryID);
                    command.Parameters.AddWithValue("@Supplier", product.Supplier ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Price", product.Price);
                    command.Parameters.AddWithValue("@StockQuantity", product.StockQuantity);
                    command.Parameters.AddWithValue("@Description", product.Description ?? (object)DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка обновления товара: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void DeleteProduct(int productId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    
                    SqlCommand deleteSalesCommand = new SqlCommand(
                        "DELETE FROM Sales WHERE ProductID = @ProductID",
                        connection,
                        transaction);
                    deleteSalesCommand.Parameters.AddWithValue("@ProductID", productId);
                    deleteSalesCommand.ExecuteNonQuery();

                    SqlCommand deleteProductCommand = new SqlCommand(
                        "DELETE FROM Products WHERE ProductID = @ProductID",
                        connection,
                        transaction);
                    deleteProductCommand.Parameters.AddWithValue("@ProductID", productId);
                    deleteProductCommand.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (SqlException ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"Ошибка удаления товара: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


    }
}
