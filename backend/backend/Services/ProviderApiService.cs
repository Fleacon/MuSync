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

    private OAuthTokensDAO oAuthTokensDao;
    private AuthService authService;

    public ProviderApiService(OAuthTokensDAO authTokensDao, AuthService authService)
    {
        oAuthTokensDao = authTokensDao;
        this.authService = authService;
    }

    public async Task<UserPlaylists> GetUserPlaylists(Provider provider, HttpContext httpContext)
    {
        if (!providers.TryGetValue(provider, out var handler))
            return new (Provider.Invalid, Array.Empty<Playlist>());
        
        if (!httpContext.Request.Cookies.TryGetValue($"AccessToken_{provider.ToString()}", out var token))
            return new (provider, Array.Empty<Playlist>());
        
        return await handler.GetUserPlaylists(token);
    }

    public async Task<ProviderAccess> GetUserData(Provider provider, string token)
    {
        if (!providers.TryGetValue(provider, out var handler))
            return new (Provider.Invalid, "Invalid", "-");
        
        return await handler.GetUserData(token);
    }

    public async Task<SearchQuery> SearchForTracks(Provider provider, string token, string search)
    {
        if (!providers.TryGetValue(provider, out var handler))
            return new (Provider.Invalid, Array.Empty<Track>());
        
        return await handler.SearchForTracks(token, search);
    }
}