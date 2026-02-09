using backend.Models;
using Google.Protobuf.Reflection;
using MySql.Data.MySqlClient;

namespace backend.DB.DAO;

public class OAuthTokensDAO
{
    private readonly DbManager db;

    public OAuthTokensDAO(DbManager db) => this.db = db;

    public async Task<OAuthToken?> GetOAuthTokenByUserId(int userId)
    {
        await using var conn = db.CreateConnection();
        await using var cmd = new MySqlCommand("SELECT * FROM OAuthTokens WHERE UserId = @user", conn);
        cmd.Parameters.AddWithValue("@user", userId);
        var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            Enum.TryParse<Provider>(reader.GetString(1), true, out var prov);
            return new OAuthToken(
                reader.GetInt32(0),
                reader.GetInt32(2),
                prov,
                reader.GetString(3),
                reader.GetInt32(4));
        }
        return null;
    }

    public async Task<OAuthToken> CreateOAuthToken(OAuthToken authToken)
    {
        await using var conn = db.CreateConnection();
        await using var cmd =
            new MySqlCommand(
                "INSERT INTO OAuthTokens(Provider, ProviderUserId, RefreshToken, UserId) VALUES (@provider, @providerUserId, @refreshToken, @userId);" +
                "SELECT LAST_INSERT_ID();");
        cmd.Parameters.AddWithValue("@provider", authToken.Provider.ToString());
        cmd.Parameters.AddWithValue("@providerUserId", authToken.ProviderUserId);
        cmd.Parameters.AddWithValue("@refreshToken", authToken.RefreshToken);
        cmd.Parameters.AddWithValue("@userId", authToken.UserId);
        
        var result = await cmd.ExecuteScalarAsync();
        authToken.OAuthId = Convert.ToInt32(result);

        return authToken;
    }
}