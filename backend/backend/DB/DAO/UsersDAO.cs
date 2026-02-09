using backend.Models;
using MySql.Data.MySqlClient;

namespace backend.DB.DAO;

public class UsersDAO
{
    private readonly DbManager db;

    public UsersDAO(DbManager db) => this.db = db;

    public async Task<User?> GetUserById(int id)
    {
        await using var conn = db.CreateConnection();
        await using var cmd = new MySqlCommand(
            "SELECT * FROM Users WHERE UserId = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new User(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
        }
        return null;
    }

    public async Task<User> CreateUser(User user)
    {
        await using var conn = db.CreateConnection();
        await using var cmd =
            new MySqlCommand("INSERT INTO Users (Email, Username, Password) VALUES (@email, @username, @password);" + "SELECT LAST_INSERT_ID();", conn);
        cmd.Parameters.AddWithValue("@email", user.Email);
        cmd.Parameters.AddWithValue("@username", user.Username);
        cmd.Parameters.AddWithValue("@password", user.PasswordHash);
        
        var result = await cmd.ExecuteScalarAsync();
        user.UserId = Convert.ToInt32(result);
        
        return user;
    }
}