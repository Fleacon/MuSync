using backend.DB.DAO;

namespace backend.Services;

public class PreferencesService
{
    private readonly PreferencesDAO preferencesDao;

    public PreferencesService(PreferencesDAO preferencesDao)
    {
        this.preferencesDao = preferencesDao;
    }

    public async Task<string?> GetPreference(int userId, string key)
    {
        var preference = await preferencesDao.GetPreferenceByUserId(userId, key);
        return preference?.Value;
    }

    public async Task<IReadOnlyDictionary<string, string>> GetAllPreferences(int userId)
    {
        var preferences = await preferencesDao.GetAllPreferencesByUserId(userId);
        return preferences.ToDictionary(p => p.Key, p => p.Value);
    }

    public async Task SetPreference(int userId, string key, string value)
    {
        await preferencesDao.SetPreferenceByUserId(userId, key, value);
    }

    public async Task<bool> DeletePreference(int userId, string key)
    {
        return await preferencesDao.DeletePreferenceByUserId(userId, key);
    }
}