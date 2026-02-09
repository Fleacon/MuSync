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
        await using var cmd = new MySqlCommand("SELECT * FROM RememberTokens WHERE RememberId = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new RememberToken(reader.GetInt32(0), reader.GetDateTime(1), reader.GetDateTime(2),
                reader.GetString(3), reader.GetInt32(4));
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
}