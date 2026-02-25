using backend.Models;
using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Http;
using SearchRequest = SpotifyAPI.Web.SearchRequest;

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

    public async Task<OAuthResult> RefreshAccessTokenAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public async Task<UserPlaylists> GetUserPlaylistsAsync(string accessToken)
    {
        throw new NotImplementedException();
    }

    public async Task<ProviderAccess> GetUserDataAsync(string accessToken)
    {
        throw new NotImplementedException();
    }

    public async Task<SearchQuery> SearchForTracksAsync(string accessToken, string query)
    {
        throw new NotImplementedException();
    }

    public async Task AddSongToPlaylistAsync(string accessToken, string trackId, string playlistId)
    {
        throw new NotImplementedException();
    }
}