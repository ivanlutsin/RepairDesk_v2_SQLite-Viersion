using YourApp.Services;

namespace YourApp.Data
{
    public class DatabaseInitializer
    {
        public static void Initialize()
        {
            var db = new DatabaseService();
            using var connection = db.GetConnection();
            connection.Open();
        }
    }
}