using backend.Models;
using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Http;

namespace backend.Providers;

public class SpotifyAPI : IProvider
{
    public Provider Provider => Provider.Spotify;
    
    private string[] scope = 
    {
        Scopes.PlaylistReadPrivate,
        Scopes.PlaylistModifyPrivate,
        Scopes.PlaylistModifyPublic,
        Scopes.UserLibraryRead,
        Scopes.UserLibraryModify
    };
    
    private readonly string clientId = Env.GetString("SPOTIFY_CLIENTID");
    private readonly string clientSecret = Env.GetString("SPOTIFY_CLIENTSECRET");
    private readonly string redirectUri = Env.GetString("SPOTIFY_REDIRECTURI");
    
    public ActionResult AuthRequest()
    {
        var loginRequest = new LoginRequest(new (redirectUri), clientId, LoginRequest.ResponseType.Code)
        {
            Scope = scope
        };

        var uri = loginRequest.ToUri();
        return new RedirectResult(uri.ToString());
    }

    public async Task<OAuthResult> HandleCallbackAsync(HttpContext httpContext)
    {
        var code = httpContext.Request.Query["code"].ToString();
        if (string.IsNullOrEmpty(code)) throw new ("Missing code");
        
        var spotifyConfig = SpotifyClientConfig.CreateDefault();

        var response = await new OAuthClient(spotifyConfig).RequestToken(
            new AuthorizationCodeTokenRequest(
                clientId,
                clientSecret,
                code,
                new (redirectUri)
            )
        );
        return new(response.RefreshToken, response.AccessToken, response.CreatedAt.AddSeconds(response.ExpiresIn));
    }

    public Task<string> RefreshAccessToken(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task<UserPlaylists> GetUserPlaylists(string accessToken)
    {
        throw new NotImplementedException();
    }

    public Task<ProviderAccess> GetUserData(string accessToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsTokenValid(string accessToken)
    {
        throw new NotImplementedException();
    }
}