using backend.Models;
using Google.Protobuf.Reflection;
using MySql.Data.MySqlClient;

namespace backend.DB.DAO;

public class OAuthTokensDAO
{
    private readonly DbManager db;

    public OAuthTokensDAO(DbManager db) => this.db = db;

    public async Task<List<OAuthToken>> GetOAuthTokenByUserId(int userId)
    {
        var tokens = new List<OAuthToken>();
        
        await using var conn = db.CreateConnection();
        await using var cmd = new MySqlCommand("SELECT * FROM OAuthTokens WHERE UserId = @user", conn);
        cmd.Parameters.AddWithValue("@user", userId);
        var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            Enum.TryParse<Provider>(reader.GetString(1), true, out var prov);
            tokens.Add(new OAuthToken(
                reader.GetInt32(0),
                prov,
                reader.GetString(3),
                reader.GetInt32(4)));
        }
        return tokens;
    }

    public async Task<OAuthToken> CreateOAuthToken(OAuthToken authToken)
    {
        await using var conn = db.CreateConnection();
        await using var cmd =
            new MySqlCommand(
                "INSERT INTO OAuthTokens(Provider, RefreshToken, UserId) VALUES (@provider, @refreshToken, @userId);" +
                "SELECT LAST_INSERT_ID();");
        cmd.Parameters.AddWithValue("@provider", authToken.Provider.ToString());
        cmd.Parameters.AddWithValue("@refreshToken", authToken.RefreshToken);
        cmd.Parameters.AddWithValue("@userId", authToken.UserId);
        
        var result = await cmd.ExecuteScalarAsync();
        authToken.OAuthId = Convert.ToInt32(result);

        return authToken;
    }
}