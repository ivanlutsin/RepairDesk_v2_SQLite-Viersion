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
        private readonly string _dbPath;

        public string DbPath => _dbPath;

        public DatabaseService()
        {
            // 📁 AppData путь
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var folder = Path.Combine(appData, "RepairDesk");

            Directory.CreateDirectory(folder);

            _dbPath = Path.Combine(folder, "SQLite_DataBase_File.db");

            // 📦 путь к шаблону (рядом с exe)
            var templatePath = Path.Combine(AppContext.BaseDirectory, "SQLite_DataBase_Template.db");

            // 🔥 если БД нет — копируем
            if (!File.Exists(_dbPath))
            {
                if (File.Exists(templatePath))
                {
                    File.Copy(templatePath, _dbPath);
                }
            }

            _connectionString = $"Data Source={_dbPath}";

            Console.WriteLine($"[DB] Path: {_dbPath}");
            Console.WriteLine($"[DB] Exists: {File.Exists(_dbPath)}");

            InitializeDatabase();
        }

        public SqliteConnection GetConnection()
        {
            return new SqliteConnection(_connectionString);
        }

        // ================= INIT =================

        private void InitializeDatabase()
        {
            using var connection = GetConnection();
            connection.Open();

            var cmd = connection.CreateCommand();

            cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS Products (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    ProductName TEXT NOT NULL,
    Barcode TEXT NOT NULL,
    Price REAL NOT NULL,
    Quantity INTEGER,
    Characteristics TEXT
);

CREATE TABLE IF NOT EXISTS Cart (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    ProductID INTEGER NOT NULL,
    ProductName TEXT NOT NULL,
    Quantity INTEGER NOT NULL,
    Price REAL NOT NULL
);

CREATE TABLE IF NOT EXISTS Repairs (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    DeviceType TEXT NOT NULL,
    Brand TEXT NOT NULL,
    ClientFullName TEXT NOT NULL,
    PhoneNumber TEXT NOT NULL,
    Model TEXT NOT NULL,
    SerialNumber TEXT,
    ProblemDescription TEXT,
    StartDate TEXT NOT NULL,
    EndDate TEXT,
    RepairsStatus TEXT,
    Order_Number NUMERIC NOT NULL
);

CREATE TABLE IF NOT EXISTS Users (
    ID INTEGER PRIMARY KEY,
    FullName TEXT,
    Role TEXT,
    SellerCode TEXT UNIQUE,
    Working_Hours TEXT
);

CREATE TABLE IF NOT EXISTS Sales (
    ID INTEGER PRIMARY KEY,
    ProductName TEXT NOT NULL,
    Barcode TEXT NOT NULL,
    PriceAtMoment REAL NOT NULL,
    TotalAmount INTEGER NOT NULL,
    SellerName TEXT NOT NULL,
    Date TEXT NOT NULL
)";

            cmd.ExecuteNonQuery();
        }

        // ================= PRODUCTS =================

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

        public bool SellProduct(Products product, int quantity = 1)
        {
            Console.WriteLine($"[DB] SellProduct: {product.ProductName}, ID={product.ID}, Qty={quantity}");
            if (product.Quantity < quantity)
            {
                Console.WriteLine($"[DB] Недостаточно товара: есть {product.Quantity}, нужно {quantity}");
                return false;
            }


            using var connection = GetConnection();

            connection.Open();


            using var tx = connection.BeginTransaction();


            try
            {
                // 1. Уменьшаем количество в Products

                var updateQuery = "UPDATE Products SET Quantity = Quantity - @qty WHERE ID = @id";
                using var updateCmd = new SqliteCommand(updateQuery, connection);
                updateCmd.Parameters.AddWithValue("@qty", quantity);
                updateCmd.Parameters.AddWithValue("@id", product.ID);
                int rows = updateCmd.ExecuteNonQuery();
                Console.WriteLine($"[DB] Products updated: {rows} rows");

                // 2. Добавляем в корзину
                var checkQuery = "SELECT ID, Quantity FROM Cart WHERE ProductID = @productId";
                using var checkCmd = new SqliteCommand(checkQuery, connection);
                checkCmd.Parameters.AddWithValue("@productId", product.ID);

                using var reader = checkCmd.ExecuteReader();
                if (reader.Read())
                {

                    int cartId = reader.GetInt32(0);
                    int newQty = reader.GetInt32(1) + quantity;
                    var updateCartQuery = "UPDATE Cart SET Quantity = @qty WHERE ID = @id";
                    using var updateCartCmd = new SqliteCommand(updateCartQuery, connection);
                    updateCartCmd.Parameters.AddWithValue("@qty", newQty);
                    updateCartCmd.Parameters.AddWithValue("@id", cartId);
                    updateCartCmd.ExecuteNonQuery();
                    Console.WriteLine($"[DB] Cart updated: new quantity={newQty}");
                }
                else
                {
                    // Вставляем новый товар в корзину

                    var insertQuery = @"INSERT INTO Cart (ProductID, ProductName, Quantity, Price)            
VALUES (@productId, @name, @qty, @price)";
                    using var insertCmd = new SqliteCommand(insertQuery, connection);
                    insertCmd.Parameters.AddWithValue("@productId", product.ID);
                    insertCmd.Parameters.AddWithValue("@name", product.ProductName);
                    insertCmd.Parameters.AddWithValue("@qty", quantity);
                    insertCmd.Parameters.AddWithValue("@price", product.Price);
                    insertCmd.ExecuteNonQuery();
                    Console.WriteLine($"[DB] New item added to cart");
                }

                tx.Commit();

                // Обновляем объект в памяти

                product.Quantity -= quantity;
                Console.WriteLine($"[DB] Success! New quantity: {product.Quantity}");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB] ERROR: {ex.Message}");
                tx.Rollback();
                return false;
            }
        }

        // ================= CART =================

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

            int? cartId = null;
            int currentQty = 0;

            using (var reader = checkCmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    cartId = reader.GetInt32(0);
                    currentQty = reader.GetInt32(1);
                }
            }

            if (cartId != null)
            {
                var updateQuery = "UPDATE Cart SET Quantity = @qty WHERE ID = @id";
                using var updateCmd = new SqliteCommand(updateQuery, connection);

                updateCmd.Parameters.AddWithValue("@qty", currentQty + quantity);
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

        public ObservableCollection<DeviceItem> LoadDevices()
        {
            var devices = new ObservableCollection<DeviceItem>();
            using var connection = GetConnection();
            connection.Open();

            var query = "SELECT ID, Device_Type, Brand, Model FROM Devices";
            using var cmd = new SqliteCommand(query, connection);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                devices.Add(new DeviceItem()
                {
                    ID = reader.GetInt32(0),
                    Device_Type = reader.GetString(1),
                    Brand = reader.GetString(2),
                    Model = reader.GetString(3)
                });
            }

            return devices;
        }

        // Добавить устройство
        public void AddDevice(DeviceItem device)
        {
            using var connection = GetConnection();
            connection.Open();

            var query = @"
        INSERT INTO Devices (Device_Type, Brand, Model)
        VALUES (@type, @brand, @model);
        SELECT last_insert_rowid();";

            using var cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@type", device.Device_Type);
            cmd.Parameters.AddWithValue("@brand", device.Brand);
            cmd.Parameters.AddWithValue("@model", device.Model);

            device.ID = Convert.ToInt32(cmd.ExecuteScalar());
        }

        // Удалить устройство
        public void DeleteDevice(int deviceId)
        {
            using var connection = GetConnection();
            connection.Open();

            var query = "DELETE FROM Devices WHERE ID = @id";
            using var cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", deviceId);
            cmd.ExecuteNonQuery();
        }



        public ObservableCollection<PhoneBookItem> LoadPhoneBook()
        {
            var items = new ObservableCollection<PhoneBookItem>();
            using var connection = GetConnection();
            connection.Open();

            var query = "SELECT ID, FullName, PhoneNumber, Comment FROM PhoneBook ORDER BY FullName";
            using var cmd = new SqliteCommand(query, connection);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                items.Add(new PhoneBookItem
                {
                    ID = reader.GetInt32(0),
                    FullName = reader.GetString(1),
                    PhoneNumber = reader.GetString(2),
                    Comment = reader.GetString(3)
                });
            }

            return items;
        }

// Добавить запись
        public void AddPhoneBookItem(PhoneBookItem item)
        {
            using var connection = GetConnection();
            connection.Open();

            var query = @"
        INSERT INTO PhoneBook (FullName, PhoneNumber, Comment)
        VALUES (@name, @phone, @comment);
        SELECT last_insert_rowid();";

            using var cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@name", item.FullName);
            cmd.Parameters.AddWithValue("@phone", item.PhoneNumber);
            cmd.Parameters.AddWithValue("@comment", item.Comment ?? "");

            item.ID = Convert.ToInt32(cmd.ExecuteScalar());
        }

// Обновить запись
        public void UpdatePhoneBookItem(PhoneBookItem item)
        {
            using var connection = GetConnection();
            connection.Open();

            var query = @"
        UPDATE PhoneBook 
        SET FullName = @name,
            PhoneNumber = @phone,
            Comment = @comment
        WHERE ID = @id";

            using var cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@name", item.FullName);
            cmd.Parameters.AddWithValue("@phone", item.PhoneNumber);
            cmd.Parameters.AddWithValue("@comment", item.Comment ?? "");
            cmd.Parameters.AddWithValue("@id", item.ID);

            cmd.ExecuteNonQuery();
        }

// Удалить запись
        public void DeletePhoneBookItem(int id)
        {
            using var connection = GetConnection();
            connection.Open();

            var query = "DELETE FROM PhoneBook WHERE ID = @id";
            using var cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public void AddClientToPhoneBookIfNotExists(string fullName, string phoneNumber, string comment = "")
        {
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(phoneNumber))
                return;

            using var connection = GetConnection();
            connection.Open();

            // Проверяем, есть ли уже такой номер
            var checkQuery = "SELECT COUNT(*) FROM PhoneBook WHERE PhoneNumber = @phone";
            using var checkCmd = new SqliteCommand(checkQuery, connection);
            checkCmd.Parameters.AddWithValue("@phone", phoneNumber);
            int count = Convert.ToInt32(checkCmd.ExecuteScalar());

            if (count == 0)
            {
                var insertQuery = @"
            INSERT INTO PhoneBook (FullName, PhoneNumber, Comment)
            VALUES (@name, @phone, @comment)";

                using var insertCmd = new SqliteCommand(insertQuery, connection);
                insertCmd.Parameters.AddWithValue("@name", fullName);
                insertCmd.Parameters.AddWithValue("@phone", phoneNumber);
                insertCmd.Parameters.AddWithValue("@comment", string.IsNullOrEmpty(comment)
                    ? "Автоматически добавлен из заказа"
                    : comment);
                insertCmd.ExecuteNonQuery();

                Console.WriteLine($"[DB] Добавлен в телефонную книгу: {fullName} ({phoneNumber})");
            }
        }
    }
}