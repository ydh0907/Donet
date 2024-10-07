using MySqlConnector;

namespace JumpUpServer
{
    public class DatabaseManager
    {
        private static string connectionData;
        private static bool dnsSrb = true;

        public static void Initialize
            (string endpoint, string user_id, string password, string database, bool dnsSrb = true)
        {
            var sb = new MySqlConnectionStringBuilder()
            {
                Server = endpoint,
                UserID = user_id,
                Password = password,
                Database = database,
            };
            connectionData = sb.ConnectionString;
            DatabaseManager.dnsSrb = dnsSrb;
        }

        public static async Task<bool> Query<T>(string queryString)
        {
            int success = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionData))
            {
                await connection.OpenAsync();
                using (MySqlCommand command = new MySqlCommand(queryString, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
            return success == 1;
        }
    }
}
