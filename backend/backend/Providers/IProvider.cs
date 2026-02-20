using backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.Providers;

public interface IProvider
{
    public Provider Provider { get; }
    public ActionResult AuthRequest();
    public Task<OAuthResult> HandleCallbackAsync(HttpContext httpContext);

    public Task<string> RefreshAccessToken(string refreshToken);

    public Task<UserPlaylists> GetUserPlaylists(string accessToken);

    public Task<ProviderAccess> GetUserData(string accessToken);

    public Task<bool> IsTokenValid(string accessToken);
}