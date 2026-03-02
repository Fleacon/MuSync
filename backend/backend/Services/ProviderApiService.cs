using backend.DB.DAO;
using backend.Models;
using backend.Providers;

namespace backend.Services;

public class ProviderApiService
{
    private Dictionary<Provider, IProvider> providers = new()
    {
        { Provider.Spotify, new Providers.SpotifyAPI()},
        { Provider.YouTubeMusic, new YoutubeAPI()}
    };

    public async Task<UserPlaylists> GetUserPlaylists(Provider provider, HttpRequest request)
    {
        if (!providers.TryGetValue(provider, out var handler))
            return new (Provider.Invalid, Array.Empty<Playlist>());
        
        if (!request.Cookies.TryGetValue($"AccessToken_{provider.ToString()}", out var token))
            return new (provider, Array.Empty<Playlist>());
        
        return await handler.GetUserPlaylistsAsync(token);
    }

    public async Task<ProviderAccess> GetUserData(Provider provider, string token)
    {
        if (!providers.TryGetValue(provider, out var handler))
            return new (Provider.Invalid, "Invalid", "-");
        
        return await handler.GetUserDataAsync(token);
    }

    public async Task<SearchQuery> SearchForTracks(Provider provider, string token, string search)
    {
        if (!providers.TryGetValue(provider, out var handler))
            return new (Provider.Invalid, Array.Empty<Track>());
        
        return await handler.SearchForTracksAsync(token, search);
    }

    public async Task AddToPlaylist(Provider provider, string token, string trackId, string playlistId)
    {
        if (!providers.TryGetValue(provider, out var handler))
            return;

        await handler.AddSongToPlaylistAsync(token, trackId, playlistId);
    }
}