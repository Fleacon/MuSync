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

    public async Task<ProviderAccess> GetUserData(OAuthToken oAuth, HttpContext httpContext)
    {
        if (!providers.TryGetValue(oAuth.Provider, out var handler))
            return new (Provider.Invalid, "Invalid");
        
        if (!httpContext.Request.Cookies.TryGetValue($"AccessToken_{oAuth.Provider.ToString()}", out var token))
            return new(oAuth.Provider, "Invalid");

        if (!await authService.IsTokenValid(oAuth.Provider, token))
        {
            var newToken = await authService.RefreshAccessToken(oAuth);
            httpContext.Response.Cookies.Append($"AccessToken_{oAuth.Provider.ToString()}", newToken);
        }
        
        return await handler.GetUserData(token);
    }

    public async Task<IReadOnlyList<ProviderAccess>> GetConnectedUserData(HttpContext httpContext)
    {
        List<ProviderAccess> providerAccesses = [];
        
        var session = httpContext.Request.Cookies["Session"];
        var tokens = await oAuthTokensDao.GetOAuthTokenByHashedSession(SessionService.HashSessionToken(session));

        foreach (var t in tokens)
        {
            providerAccesses.Add(await GetUserData(t, httpContext));
        }

        return providerAccesses;
    }
}