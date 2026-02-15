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
    
    public ActionResult AuthRequest(HttpContext httpContext)
    {
        var redirectUri = Env.GetString("SPOTIFY_REDIRECTURI");
        var loginRequest = new LoginRequest(new (redirectUri), Env.GetString("SPOTIFY_CLIENTID"), LoginRequest.ResponseType.Code)
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

        var redirectUri = Env.GetString("SPOTIFY_REDIRECTURI");
        var spotifyConfig = SpotifyClientConfig.CreateDefault();

        var response = await new OAuthClient(spotifyConfig).RequestToken(
            new AuthorizationCodeTokenRequest(
                Env.GetString("SPOTIFY_CLIENTID"),
                Env.GetString("SPOTIFY_CLIENTSECRET"),
                code,
                new (redirectUri)
            )
        );
        return new(response.RefreshToken, response.AccessToken, response.CreatedAt.AddSeconds(response.ExpiresIn));
    }
}