using System.Data.Common;
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
            "SELECT UserId, Username, Password FROM Users WHERE UserId = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapUser(reader);
        }
        return null;
    }
    
    public async Task<User?> GetUserByUsername(string name)
    {
        await using var conn = db.CreateConnection();
        await using var cmd = new MySqlCommand(
            "SELECT UserId, Username, Password FROM Users WHERE Username = @name", conn);
        cmd.Parameters.AddWithValue("@name", name);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapUser(reader);
        }
        return null;
    }
    
    public async Task<User?> GetUserByHashedSessionToken(string token)
    {
        await using var conn = db.CreateConnection();
        await using var cmd = new MySqlCommand(
            "SELECT UserId, Username, Password FROM Users JOIN Sessions USING(UserId) WHERE SessionHash = @token", conn);
        cmd.Parameters.AddWithValue("@token", token);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapUser(reader);
        }
        return null;
    }

    public async Task<User?> GetUserByRememberHash(string hash)
    {
        await using var conn = db.CreateConnection();
        await using var cmd = new MySqlCommand(
            "SELECT Users.UserId, Username, Password FROM Users " +
            "JOIN RememberTokens USING(UserId) WHERE TokenHash = @hash AND ExpiryDate > @now", conn);
        cmd.Parameters.AddWithValue("@hash", hash);
        cmd.Parameters.AddWithValue("@now", DateTime.Now);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
            return MapUser(reader);
        return null;
    }
    
    public async Task<User> CreateUser(User user)
    {
        await using var conn = db.CreateConnection();
        await using var cmd =
            new MySqlCommand("INSERT INTO Users (Username, Password) VALUES (@username, @password);" + "SELECT LAST_INSERT_ID();", conn);
        cmd.Parameters.AddWithValue("@username", user.Username);
        cmd.Parameters.AddWithValue("@password", user.PasswordHash);
        
        var result = await cmd.ExecuteScalarAsync();
        user.UserId = Convert.ToInt32(result);
        
        return user;
    }
    
    private static User MapUser(DbDataReader reader)
    {
        return new (
            reader.GetInt32(reader.GetOrdinal("UserId")),
            reader.GetString(reader.GetOrdinal("Username")),
            reader.GetString(reader.GetOrdinal("Password"))
        );
    }
}