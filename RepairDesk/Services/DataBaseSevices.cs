using System;
using Microsoft.Data.Sqlite;
using System.IO;

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
            
            File.WriteAllText("path.txt", _dbPath);
            _connectionString = $"Data Source={_dbPath}";
            
            Console.WriteLine($"[DB] Path: {_dbPath}");
            Console.WriteLine($"[DB] File exists: {File.Exists(_dbPath)}");
        }

        public SqliteConnection GetConnection()
        {
            return new SqliteConnection(_connectionString);
        }
    }
}