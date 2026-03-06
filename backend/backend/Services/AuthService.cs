using backend.DB.DAO;
using backend.Models;
using backend.Providers;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace backend.Services;

public class AuthService
{
    private readonly ProviderRegistry registry;
    private readonly IDataProtector protector;
    private readonly OAuthTokensDAO oAuthTokensDao;

    public AuthService(OAuthTokensDAO oAuthDao, IDataProtectionProvider protector, ProviderRegistry registry)
    {
        oAuthTokensDao = oAuthDao;
        this.protector = protector.CreateProtector("Provider-Refresh-Token");
        this.registry = registry;
    }

    public ActionResult RequestAuth(Provider prov, HttpContext httpContext)
    {
        if (!registry.TryGet(prov, out var handler))
            return new BadRequestResult();

        return handler.AuthRequest(httpContext);
    }

    public async Task<OAuthResult?> HandleCallback(Provider provider, HttpContext httpContext)
    {
        if (!registry.TryGet(provider, out var handler))
            return null;

        return await handler.HandleCallbackAsync(httpContext);
    }

    public async Task<OAuthResult?> RefreshAccessToken(Provider provider, User user)
    {
        if (!registry.TryGet(provider, out var handler))
            return null;
        var userProviders = await oAuthTokensDao.GetOAuthTokenByUserId(user.UserId);
        var oAuthToken = userProviders
            .First(p => p.Provider == provider);
        
        var refreshToken = protector.Unprotect(oAuthToken.RefreshToken);
        var newToken = await handler.RefreshAccessTokenAsync(refreshToken);

        var encryptedRefreshToken = protector.Protect(newToken.RefreshToken);
        await oAuthTokensDao.UpdateRefreshTokenById(oAuthToken, encryptedRefreshToken);
        
        return new(newToken.RefreshToken, newToken.AccessToken, newToken.Expiry);
    }

    public async Task CreateOAuthToken(Provider provider, OAuthResult result, int userId)
    {
        var refreshToken = protector.Protect(result.RefreshToken);
        await oAuthTokensDao.CreateOAuthToken(new(0, provider, refreshToken, userId));
    }
}