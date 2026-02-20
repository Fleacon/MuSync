using backend.DB.DAO;
using backend.Models;
using backend.Providers;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace backend.Services;

public class AuthService
{

    private Dictionary<Provider, IProvider> providers = new()
    {
        { Provider.Spotify, new Providers.SpotifyAPI()},
        { Provider.YouTubeMusic, new YoutubeAPI()}
    };

    private readonly IDataProtector protector;

    private OAuthTokensDAO oAuthTokensDao;
    
    public AuthService(OAuthTokensDAO oAuthDao, IDataProtectionProvider protector)
    {
        oAuthTokensDao = oAuthDao;
        this.protector = protector.CreateProtector("Provider-Refresh-Token");
    }

    public ActionResult RequestAuth(Provider prov)
    {
        if (!providers.TryGetValue(prov, out var handler))
            return new BadRequestResult();

        return handler.AuthRequest();
    }
    
    public async Task<OAuthResult?> HandleCallback(Provider provider, HttpContext httpContext)
    {
        if (!providers.TryGetValue(provider, out var handler))
            return null;

        return await handler.HandleCallbackAsync(httpContext);
    }

    public async Task<string?> RefreshAccessToken(Provider provider, User user)
    {
        if (!providers.TryGetValue(provider, out var handler))
            return null;
        var userProviders = await oAuthTokensDao.GetOAuthTokenByUserId(user.UserId);
        var encryptedRefreshToken = userProviders
            .First(p => p.Provider == provider)
            .RefreshToken;
        var refreshToken = protector.Unprotect(encryptedRefreshToken);
        var newAccessToken = await handler.RefreshAccessToken(refreshToken);
        return newAccessToken;
    }
    
    public async Task<string?> RefreshAccessToken(OAuthToken oAuth)
    {
        if (!providers.TryGetValue(oAuth.Provider, out var handler))
            return null;
        var refreshToken = protector.Unprotect(oAuth.RefreshToken);
        var newAccessToken = await handler.RefreshAccessToken(refreshToken);
        return newAccessToken;
    }
    
    public async Task CreateOAuthToken(Provider provider, OAuthResult result, int userId)
    {
        var refreshToken = protector.Protect(result.RefreshToken);
        
        await oAuthTokensDao.CreateOAuthToken(new(0, provider, refreshToken, userId));
    }

    public async Task<bool> IsTokenValid(Provider provider, string token)
    {
        if (!providers.TryGetValue(provider, out var handler))
            return false;
        
        return await handler.IsTokenValid(token);
    }
}