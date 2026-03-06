using System.Data.Common;
using backend.Models;
using MySql.Data.MySqlClient;

namespace backend.DB.DAO;

public class PreferencesDAO
{
    private readonly DbManager db;

    public PreferencesDAO(DbManager db) => this.db = db;
    
    public async Task<UserPreference?> GetPreferenceByUserId(int userId, string key)
    {
        await using var conn = db.CreateConnection();
        await using var cmd = new MySqlCommand(
            "SELECT UserId, PreferenceKey, PreferenceValue FROM UserPreferences " +
            "WHERE UserId = @userId AND PreferenceKey = @key", conn);
        cmd.Parameters.AddWithValue("@userId", userId);
        cmd.Parameters.AddWithValue("@key", key);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
            return MapPreference(reader);
        return null;
    }

    public async Task<List<UserPreference>> GetAllPreferencesByUserId(int userId)
    {
        var preferences = new List<UserPreference>();

        await using var conn = db.CreateConnection();
        await using var cmd = new MySqlCommand(
            "SELECT UserId, PreferenceKey, PreferenceValue FROM UserPreferences " +
            "WHERE UserId = @userId", conn);
        cmd.Parameters.AddWithValue("@userId", userId);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            preferences.Add(MapPreference(reader));

        return preferences;
    }

    public async Task SetPreferenceByUserId(int userId, string key, string value)
    {
        await using var conn = db.CreateConnection();
        await using var cmd = new MySqlCommand(
            "INSERT INTO UserPreferences (UserId, PreferenceKey, PreferenceValue) " +
            "VALUES (@userId, @key, @value) " +
            "ON DUPLICATE KEY UPDATE PreferenceValue = @value", conn);
        cmd.Parameters.AddWithValue("@userId", userId);
        cmd.Parameters.AddWithValue("@key", key);
        cmd.Parameters.AddWithValue("@value", value);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<bool> DeletePreferenceByUserId(int userId, string key)
    {
        await using var conn = db.CreateConnection();
        await using var cmd = new MySqlCommand(
            "DELETE FROM UserPreferences WHERE UserId = @userId AND PreferenceKey = @key", conn);
        cmd.Parameters.AddWithValue("@userId", userId);
        cmd.Parameters.AddWithValue("@key", key);
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    private static UserPreference MapPreference(DbDataReader reader)
    {
        return new(
            reader.GetInt32(reader.GetOrdinal("UserId")),
            reader.GetString(reader.GetOrdinal("PreferenceKey")),
            reader.GetString(reader.GetOrdinal("PreferenceValue"))
        );
    }
}