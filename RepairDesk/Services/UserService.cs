using Dapper;
using Microsoft.Data.Sqlite;
using RepairDesk.Models;
using YourApp.Services;
using System;

namespace RepairDesk.Services
{
    public class UserService
    {
        private readonly DatabaseService _db;

        public UserService()
        {
            _db = new DatabaseService();
        }

        public User? GetBySellerCode(string code)
        {
            Console.WriteLine($"[UserService] Code search: '{code}'");
            Console.WriteLine($"[UserService] Database path: {_db.DbPath}");
            
            using var connection = _db.GetConnection();
            connection.Open();
            
            Console.WriteLine($"[UserService] Connection state: {connection.State}");

            // Проверка: какие таблицы есть
            var tables = connection.Query<string>("SELECT name FROM sqlite_master WHERE type='table'");
            Console.WriteLine($"[UserService] Tables in DB: [{string.Join(", ", tables)}]");

            // Проверка: есть ли таблица Users
            var tableExists = connection.ExecuteScalar<long>(
                "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='Users'");
            Console.WriteLine($"[UserService] Users table exists: {tableExists > 0}");

            // Проверка: сколько записей в Users
            if (tableExists > 0)
            {
                var userCount = connection.ExecuteScalar<long>("SELECT COUNT(*) FROM Users");
                Console.WriteLine($"[UserService] Users count: {userCount}");

                // Вывод всех пользователей
                var allUsers = connection.Query("SELECT * FROM Users");
                foreach (var u in allUsers)
                {
                    Console.WriteLine($"[UserService] User: {u}");
                }

                // Вывод всех SellerCode
                var allCodes = connection.Query<string>("SELECT SellerCode FROM Users");
                Console.WriteLine($"[UserService] All SellerCodes: [{string.Join(", ", allCodes)}]");
            }

            // Поиск пользователя
            var user = connection.QueryFirstOrDefault<User>(
                "SELECT Id, FullName, Role, SellerCode FROM Users WHERE SellerCode = @Code",
                new { Code = code });
            
            if (user != null)
            {
                Console.WriteLine($"[UserService] FOUND: {user.FullName} ({user.Role})");
            }
            else
            {
                Console.WriteLine($"[UserService] NOT FOUND");
            }

            return user;
        }
    }
}