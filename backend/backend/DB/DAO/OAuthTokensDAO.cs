using System.Data.Common;
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
        await using var cmd = new MySqlCommand("SELECT OAuthId, Provider, RefreshToken, UserId FROM OAuthTokens WHERE UserId = @user", conn);
        cmd.Parameters.AddWithValue("@user", userId);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tokens.Add(MapOAuthToken(reader));
        }
        return tokens;
    }
    
    public async Task<List<OAuthToken>> GetOAuthTokenByHashedSession(string session)
    {
        var tokens = new List<OAuthToken>();
        
        await using var conn = db.CreateConnection();
        await using var cmd = new MySqlCommand("SELECT OAuthTokens.OAuthId, OAuthTokens.Provider, OAuthTokens.RefreshToken, OAuthTokens.UserId FROM OAuthTokens JOIN Sessions USING(UserId) WHERE SessionHash = @session", conn);
        cmd.Parameters.AddWithValue("@session", session);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tokens.Add(MapOAuthToken(reader));
        }
        return tokens;
    }

    public async Task<OAuthToken> CreateOAuthToken(OAuthToken authToken)
    {
        await using var conn = db.CreateConnection();
        await using var cmd =
            new MySqlCommand(
                "INSERT INTO OAuthTokens(Provider, RefreshToken, UserId) VALUES (@provider, @refreshToken, @userId);" +
                "SELECT LAST_INSERT_ID();", conn);
        cmd.Parameters.AddWithValue("@provider", authToken.Provider.ToString());
        cmd.Parameters.AddWithValue("@refreshToken", authToken.RefreshToken);
        cmd.Parameters.AddWithValue("@userId", authToken.UserId);
        
        var result = await cmd.ExecuteScalarAsync();
        authToken.OAuthId = Convert.ToInt32(result);

        return authToken;
    }

    private static OAuthToken MapOAuthToken(DbDataReader reader)
    {
        Enum.TryParse<Provider>(
            reader.GetString(reader.GetOrdinal("Provider")),
            true,
            out var provider);

        return new (
            reader.GetInt32(reader.GetOrdinal("OAuthId")),
            provider,
            reader.GetString(reader.GetOrdinal("RefreshToken")),
            reader.GetInt32(reader.GetOrdinal("UserId"))
        );
    }
}