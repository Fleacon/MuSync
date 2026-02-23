using System.Data.Common;
using backend.Models;
using MySql.Data.MySqlClient;

namespace backend.DB.DAO;

public class RememberTokensDAO
{
    private readonly DbManager db;

    public RememberTokensDAO(DbManager db) => this.db = db;

    public async Task<RememberToken?>GetRememberTokenById(int id)
    {
        await using var conn = db.CreateConnection();
        await using var cmd = new MySqlCommand("SELECT RememberId, CreationDate, ExpiryDate, TokenHash, UserId FROM RememberTokens WHERE RememberId = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapRememberToken(reader);
        }
        return null;
    }

    public async Task<RememberToken>CreateRememberToken(RememberToken token)
    {
        await using var conn = db.CreateConnection();
        await using var cmd =
            new MySqlCommand(
                "INSERT INTO RememberTokens(CreationDate, ExpiryDate, TokenHash, UserId) VALUES (@cDate, @eDate, @hash, @userId);" +
                "SELECT LAST_INSERT_ID();", conn);
        cmd.Parameters.AddWithValue("@cDate", token.CreationDate);
        cmd.Parameters.AddWithValue("@eDate", token.ExpiryDate);
        cmd.Parameters.AddWithValue("@hash", token.TokenHash);
        cmd.Parameters.AddWithValue("@userId", token.UserId);
        
        var result = await cmd.ExecuteScalarAsync();
        token.RememberId = Convert.ToInt32(result);

        return token;
    }
    
    private static RememberToken MapRememberToken(DbDataReader reader)
    {
        return new (
            reader.GetInt32(reader.GetOrdinal("RememberId")),
            reader.GetDateTime(reader.GetOrdinal("CreationDate")),
            reader.GetDateTime(reader.GetOrdinal("ExpiryDate")),
            reader.GetString(reader.GetOrdinal("TokenHash")),
            reader.GetInt32(reader.GetOrdinal("UserId"))
        );
    }
}