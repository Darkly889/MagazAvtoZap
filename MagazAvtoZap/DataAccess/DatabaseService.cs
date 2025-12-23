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
        public void AddOrder(Order order)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        
                        SqlCommand orderCommand = new SqlCommand(
                            "INSERT INTO Orders (OrderNumber, FullName, Phone, Address, PaymentMethod, OrderDate, Status) " +
                            "VALUES (@OrderNumber, @FullName, @Phone, @Address, @PaymentMethod, @OrderDate, @Status)",
                            connection, transaction);

                        orderCommand.Parameters.AddWithValue("@OrderNumber", order.OrderNumber);
                        orderCommand.Parameters.AddWithValue("@FullName", order.FullName);
                        orderCommand.Parameters.AddWithValue("@Phone", order.Phone);
                        orderCommand.Parameters.AddWithValue("@Address", order.Address);
                        orderCommand.Parameters.AddWithValue("@PaymentMethod", order.PaymentMethod);
                        orderCommand.Parameters.AddWithValue("@OrderDate", order.OrderDate);
                        orderCommand.Parameters.AddWithValue("@Status", order.Status);

                        orderCommand.ExecuteNonQuery();

                       
                        foreach (var item in order.Items)
                        {
                            SqlCommand orderItemCommand = new SqlCommand(
                                "INSERT INTO OrderItems (OrderNumber, ProductID, Quantity, Price) " +
                                "VALUES (@OrderNumber, @ProductID, @Quantity, @Price)",
                                connection, transaction);

                            orderItemCommand.Parameters.AddWithValue("@OrderNumber", order.OrderNumber);
                            orderItemCommand.Parameters.AddWithValue("@ProductID", item.ProductId);
                            orderItemCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                            orderItemCommand.Parameters.AddWithValue("@Price", item.Price);

                            orderItemCommand.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (SqlException ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Ошибка добавления заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка добавления заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public List<Order> GetOrderHistory()
        {
            var orders = new List<Order>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(
                        "SELECT OrderNumber, FullName, Phone, Address, PaymentMethod, OrderDate, Status FROM Orders ORDER BY OrderDate DESC",
                        connection);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var order = new Order
                            {
                                OrderNumber = reader.GetString(0),
                                FullName = reader.GetString(1),
                                Phone = reader.GetString(2),
                                Address = reader.GetString(3),
                                PaymentMethod = reader.GetString(4),
                                OrderDate = reader.GetDateTime(5),
                                Status = reader.GetString(6),
                                Items = new List<CartItem>()
                            };
                            orders.Add(order);
                        }
                    }

                    foreach (var order in orders)
                    {
                        SqlCommand itemsCommand = new SqlCommand(
                            @"SELECT oi.ProductID, oi.Quantity, oi.Price, p.Name
                      FROM OrderItems oi
                      JOIN Products p ON oi.ProductID = p.ProductID
                      WHERE oi.OrderNumber = @OrderNumber",
                            connection);
                        itemsCommand.Parameters.AddWithValue("@OrderNumber", order.OrderNumber);

                        using (SqlDataReader itemsReader = itemsCommand.ExecuteReader())
                        {
                            while (itemsReader.Read())
                            {
                                order.Items.Add(new CartItem
                                {
                                    ProductId = itemsReader.GetInt32(0),
                                    Quantity = itemsReader.GetInt32(1),
                                    Price = itemsReader.GetDecimal(2),
                                    Name = itemsReader.GetString(3)
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка загрузки истории заказов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return orders;
        }
        public Order GetOrderDetails(string orderNumber)
        {
            Order order = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(
                        "SELECT OrderNumber, FullName, Phone, Address, PaymentMethod, OrderDate, Status FROM Orders WHERE OrderNumber = @OrderNumber",
                        connection);
                    command.Parameters.AddWithValue("@OrderNumber", orderNumber);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            order = new Order
                            {
                                OrderNumber = reader.GetString(0),
                                FullName = reader.GetString(1),
                                Phone = reader.GetString(2),
                                Address = reader.GetString(3),
                                PaymentMethod = reader.GetString(4),
                                OrderDate = reader.GetDateTime(5),
                                Status = reader.GetString(6),
                                Items = new List<CartItem>()
                            };
                        }
                    }

                    if (order != null)
                    {
                        SqlCommand itemsCommand = new SqlCommand(
                            @"SELECT oi.ProductID, oi.Quantity, oi.Price, p.Name
                      FROM OrderItems oi
                      JOIN Products p ON oi.ProductID = p.ProductID
                      WHERE oi.OrderNumber = @OrderNumber",
                            connection);
                        itemsCommand.Parameters.AddWithValue("@OrderNumber", orderNumber);

                        using (SqlDataReader itemsReader = itemsCommand.ExecuteReader())
                        {
                            while (itemsReader.Read())
                            {
                                order.Items.Add(new CartItem
                                {
                                    ProductId = itemsReader.GetInt32(0),
                                    Quantity = itemsReader.GetInt32(1),
                                    Price = itemsReader.GetDecimal(2),
                                    Name = itemsReader.GetString(3)
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка загрузки деталей заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return order;
        }

        public void UpdateOrderStatus(string orderNumber, string status)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    try
                    {
                       
                        SqlCommand updateStatusCommand = new SqlCommand(
                            "UPDATE Orders SET Status = @Status WHERE OrderNumber = @OrderNumber",
                            connection, transaction);
                        updateStatusCommand.Parameters.AddWithValue("@Status", status);
                        updateStatusCommand.Parameters.AddWithValue("@OrderNumber", orderNumber);
                        updateStatusCommand.ExecuteNonQuery();

                        
                        List<(int ProductID, int Quantity)> orderItems = new List<(int, int)>();

                        using (SqlCommand getOrderItemsCommand = new SqlCommand(
                            "SELECT ProductID, Quantity FROM OrderItems WHERE OrderNumber = @OrderNumber",
                            connection, transaction))
                        {
                            getOrderItemsCommand.Parameters.AddWithValue("@OrderNumber", orderNumber);

                            using (SqlDataReader reader = getOrderItemsCommand.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    int productId = reader.GetInt32(0);
                                    int quantity = reader.GetInt32(1);
                                    orderItems.Add((productId, quantity));
                                }
                            }
                        }

                        
                        if (status == "Завершен")
                        {
                            foreach (var item in orderItems)
                            {
                                SqlCommand updateStockCommand = new SqlCommand(
                                    "UPDATE Products SET StockQuantity = StockQuantity - @Quantity WHERE ProductID = @ProductID",
                                    connection, transaction);
                                updateStockCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                                updateStockCommand.Parameters.AddWithValue("@ProductID", item.ProductID);
                                updateStockCommand.ExecuteNonQuery();
                            }
                        }
                      
                        else if (status == "Отменен")
                        {
                            foreach (var item in orderItems)
                            {
                                SqlCommand updateStockCommand = new SqlCommand(
                                    "UPDATE Products SET StockQuantity = StockQuantity + @Quantity WHERE ProductID = @ProductID",
                                    connection, transaction);
                                updateStockCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                                updateStockCommand.Parameters.AddWithValue("@ProductID", item.ProductID);
                                updateStockCommand.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch (SqlException ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Ошибка обновления статуса заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка обновления статуса заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}
