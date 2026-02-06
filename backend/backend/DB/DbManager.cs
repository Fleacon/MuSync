using MySql.Data;
using MySql.Data.MySqlClient;

namespace backend.DB;

public class DbManager
{
    private readonly string connString;

    public DbManager()
    {
        
    }

    public DbManager(string connString)
    {
        this.connString = connString;
    }

    public MySqlConnection CreateConnection()
    {
        var conn = new MySqlConnection(connString);
        conn.Open();
        return conn;
    }
}