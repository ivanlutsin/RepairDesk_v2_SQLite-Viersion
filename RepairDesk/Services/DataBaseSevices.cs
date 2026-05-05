using System;
using System.Collections.ObjectModel;
using Microsoft.Data.Sqlite;
using System.IO;
using RepairDesk.Models;

namespace YourApp.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;
        public string DbPath => _dbPath;
        private readonly string _dbPath;

        public DatabaseService()
        {
            var projectDir = @"D:\Software\RepairDesk\RepairDesk";
            _dbPath = Path.Combine(projectDir, "database", "SQLite_DataBase_File.db");

            _connectionString = $"Data Source={_dbPath};Cache=Shared";
            
            File.WriteAllText("path.txt", _dbPath);
            _connectionString = $"Data Source={_dbPath}";

            Console.WriteLine($"[DB] Path: {_dbPath}");
            Console.WriteLine($"[DB] File exists: {File.Exists(_dbPath)}");
        }

        public SqliteConnection GetConnection()
        {
            return new SqliteConnection(_connectionString);
        }

        public ObservableCollection<Products> LoadProducts()
        {
            var products = new ObservableCollection<Products>();
            using var connection = GetConnection();
            connection.Open();

            var query = "SELECT ID, ProductName, Barcode, Price, Quantity, Characteristics FROM Products";
            using var cmd = new SqliteCommand(query, connection);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                products.Add(new Products
                {
                    ID = reader.GetInt32(0),
                    ProductName = reader.GetString(1),
                    Barcode = reader.GetString(2),
                    Price = reader.GetDouble(3),
                    Quantity = reader.GetInt32(4),
                    Characteristics = reader.GetString(5)
                });
            }

            return products;
        }

        public void AddProduct(Products product)
        {
            using var connection = GetConnection();
            connection.Open();

            var query = @"
                INSERT INTO Products (ProductName, Barcode, Price, Quantity, Characteristics)
                VALUES (@name, @barcode, @price, @quantity, @char)";

            using var cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@name", product.ProductName);
            cmd.Parameters.AddWithValue("@barcode", product.Barcode);
            cmd.Parameters.AddWithValue("@price", product.Price);
            cmd.Parameters.AddWithValue("@quantity", product.Quantity);
            cmd.Parameters.AddWithValue("@char", product.Characteristics);
            cmd.ExecuteNonQuery();
        }

        public void DeleteProduct(int productId)
        {
            using var connection = GetConnection();
            connection.Open();

            var query = "DELETE FROM Products WHERE ID = @id";
            using var cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", productId);
            cmd.ExecuteNonQuery();
        }

        public void SellProduct(Products product, int quantity = 1)
        {
            if (product.Quantity < quantity) return;

            product.Quantity -= quantity;

            using var connection = GetConnection();
            connection.Open();

            var query = "UPDATE Products SET Quantity = @qty WHERE ID = @id";
            using var cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@qty", product.Quantity);
            cmd.Parameters.AddWithValue("@id", product.ID);
            cmd.ExecuteNonQuery();
        }
        // Загрузить все товары из корзины
        public ObservableCollection<CartItem> LoadCart()
        { 
            var items = new ObservableCollection<CartItem>();
            using var connection = GetConnection();
            connection.Open();
    
            var query = "SELECT ID, ProductID, ProductName, Quantity, Price FROM Cart";
            using var cmd = new SqliteCommand(query, connection);
            using var reader = cmd.ExecuteReader();
    
            while (reader.Read())
            {
                items.Add(new CartItem
            {
                ID = reader.GetInt32(0),
                ProductID = reader.GetInt32(1),
                ProductName = reader.GetString(2),
                Quantity = reader.GetInt32(3),
                Price = reader.GetDouble(4) 
            }); 
            } 
            return items; 
        }

        public void AddToCart(Products product, int quantity = 1) 
        { 
            using var connection = GetConnection(); 
            connection.Open();
            
            var checkQuery = "SELECT ID, Quantity FROM Cart WHERE ProductID = @productId"; 
            using var checkCmd = new SqliteCommand(checkQuery, connection); 
            checkCmd.Parameters.AddWithValue("@productId", product.ID);
            
            using var reader = checkCmd.ExecuteReader(); 
            if (reader.Read()) 
            { 
                int cartId = reader.GetInt32(0); 
                int newQuantity = reader.GetInt32(1) + quantity;
                
                var updateQuery = "UPDATE Cart SET Quantity = @qty WHERE ID = @id"; 
                using var updateCmd = new SqliteCommand(updateQuery, connection);
        
                updateCmd.Parameters.AddWithValue("@qty", newQuantity); 
                updateCmd.Parameters.AddWithValue("@id", cartId); 
                updateCmd.ExecuteNonQuery(); 
            }
            else 
            { 
                var insertQuery = @"
INSERT INTO Cart (ProductID, ProductName, Quantity, Price)
            VALUES (@productId, @name, @qty, @price)"; 
                using var insertCmd = new SqliteCommand(insertQuery, connection); 
                insertCmd.Parameters.AddWithValue("@productId", product.ID);
                insertCmd.Parameters.AddWithValue("@name", product.ProductName);
                insertCmd.Parameters.AddWithValue("@qty", quantity);
                insertCmd.Parameters.AddWithValue("@price", product.Price);
                insertCmd.ExecuteNonQuery(); 
            } 
        }
        public void RemoveFromCart(int cartId) 
        { 
            using var connection = GetConnection(); 
            connection.Open();
            var query = "DELETE FROM Cart WHERE ID = @id"; 
            using var cmd = new SqliteCommand(query, connection); 
            cmd.Parameters.AddWithValue("@id", cartId); 
            cmd.ExecuteNonQuery(); 
        }

        
        public void ClearCart() 
        { 
            using var connection = GetConnection(); 
            connection.Open();
            
            var query = "DELETE FROM Cart"; 
            using var cmd = new SqliteCommand(query, connection); 
            cmd.ExecuteNonQuery(); 
        }
    }
}