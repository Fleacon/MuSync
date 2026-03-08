using System.Data.Common;
using backend.Models;
using MySql.Data.MySqlClient;

namespace backend.DB.DAO;

public class SessionsDAO
{
    private readonly DbManager db;

    public SessionsDAO(DbManager db) => this.db = db;

    public async Task<Session?> GetSessionById(int id)
    {
        await using var conn = db.CreateConnection();
        await using var cmd = new MySqlCommand("SELECT SessionId, CreationDate, ExpiryDate, UserId, SessionHash FROM Sessions WHERE SessionId = @sessionId", conn);
        cmd.Parameters.AddWithValue("@sessionId", id);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapSession(reader);
        }
        return null;
    }

    public async Task<List<Session>> GetSessionByUserId(int id)
    {
        await using var conn = db.CreateConnection();
        await using var cmd = new MySqlCommand("SELECT SessionId, CreationDate, ExpiryDate, UserId, SessionHash FROM Sessions WHERE UserId = @userId", conn);
        cmd.Parameters.AddWithValue("@userId", id);
        await using var reader = await cmd.ExecuteReaderAsync();
        
        var sessions = new List<Session>();
        while (await reader.ReadAsync())
        {
            sessions.Add(MapSession(reader));
        }
        
        return sessions;
    }

    public async Task<Session?> GetSessionByHash(string hash)
    {
        await using var conn = db.CreateConnection();
        await using var cmd = new MySqlCommand("SELECT SessionId, CreationDate, ExpiryDate, UserId, SessionHash FROM Sessions WHERE SessionHash = @hash", conn);
        cmd.Parameters.AddWithValue("@hash", hash);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapSession(reader);
        }
        return null;
    }
    
    public async Task<Session> CreateSession(Session session)
    {
        await using var conn = db.CreateConnection();
        await using var cmd =
            new MySqlCommand(
                "INSERT INTO Sessions(CreationDate, ExpiryDate, UserId, SessionHash) VALUES (@cDate, @eDate, @userId, @sessionHash);" + "SELECT LAST_INSERT_ID();", conn);
        cmd.Parameters.AddWithValue("@cDate", session.CreationDate);
        cmd.Parameters.AddWithValue("@eDate", session.ExpiryDate);
        cmd.Parameters.AddWithValue("@userId", session.UserId);
        cmd.Parameters.AddWithValue("@SessionHash", session.SessionHash);
        
        var result = await cmd.ExecuteScalarAsync();
        session.SessionId = Convert.ToInt32(result);

        return session;
    }

    public async Task DeleteSessionById(int sessionId)
    {
        await using var conn = db.CreateConnection();
        await using var cmd = new MySqlCommand("DELETE FROM Sessions WHERE SessionId = @sessionId", conn);
        cmd.Parameters.AddWithValue("@sessionId", sessionId);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<Session?> UpdateExpiryDateById(int sessionId, DateTime newExpiry)
    {
        await using var conn = db.CreateConnection();
        await using var cmd = new MySqlCommand("UPDATE Sessions SET ExpiryDate = @newExpiry WHERE SessionId = @sessionId", conn);
        cmd.Parameters.AddWithValue("@newExpiry", newExpiry);
        cmd.Parameters.AddWithValue("@sessionId", sessionId);
        await cmd.ExecuteNonQueryAsync();

        return await GetSessionById(sessionId);
    }
    
    private static Session MapSession(DbDataReader reader)
    {
        return new (
            reader.GetInt32(reader.GetOrdinal("SessionId")),
            reader.GetDateTime(reader.GetOrdinal("CreationDate")),
            reader.GetDateTime(reader.GetOrdinal("ExpiryDate")),
            reader.GetInt32(reader.GetOrdinal("UserId")),
            reader.GetString(reader.GetOrdinal("SessionHash"))
        );
    }
}