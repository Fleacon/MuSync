using MySql.Data;
using MySql.Data.MySqlClient;

namespace backend.DB;

public class DbManager
{
    private readonly string connString;
    
    public DbManager(string connString)
    {
        this.connString = connString;
    }

    public MySqlConnection CreateConnection()
    {
        try
        {
            var conn = new MySqlConnection(connString);
            conn.Open();
            return conn;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseUnavailableException("Could not connect to database.", ex);
        }
    }
}

public class DatabaseUnavailableException : Exception
{
    public DatabaseUnavailableException(string message, Exception inner) : base(message, inner) { }
}