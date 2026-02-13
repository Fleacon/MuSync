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
        await using var cmd = new MySqlCommand("SELECT * FROM Sessions WHERE SessionId = @sessionId", conn);
        cmd.Parameters.AddWithValue("@sessionId", id);
        var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Session(reader.GetInt32(0), reader.GetDateTime(1), reader.GetDateTime(2), reader.GetInt32(3), reader.GetString(4));
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

    public async Task RemoveSessionById(int sessionId)
    {
        await using var conn = db.CreateConnection();
        await using var cmd = new MySqlCommand("DELETE FROM Sessions WHERE SessionId = @sessionId", conn);
        cmd.Parameters.AddWithValue("@sessionId", sessionId);
        await cmd.ExecuteScalarAsync();
    }
}