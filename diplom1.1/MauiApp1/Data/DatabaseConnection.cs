
using MySqlConnector;


namespace MauiApp1;

public class DatabaseConnection

{
    private readonly string _connectionString;

    public DatabaseConnection(string server, string database, string username, string password)
    {
        _connectionString = $"Server={server};Database={database};User ID={username};Password={password};";
    }

    public MySqlConnection GetConnection()
    {
        return new MySqlConnection(_connectionString);
    }
}
