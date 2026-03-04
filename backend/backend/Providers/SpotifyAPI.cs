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
        Scopes.UserLibraryModify,
        Scopes.PlaylistReadCollaborative
    };
    
    private readonly string clientId = Env.GetString("SPOTIFY_CLIENTID");
    private readonly string clientSecret = Env.GetString("SPOTIFY_CLIENTSECRET");
    private readonly string redirectUri = Env.GetString("SPOTIFY_REDIRECTURI");
    
    public ActionResult AuthRequest(HttpContext httpContext)
    {
        var loginRequest = new LoginRequest(new (redirectUri), clientId, LoginRequest.ResponseType.Code)
        {
            Scope = scope,
            ShowDialog = true,
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
        var newToken = await new OAuthClient().RequestToken(
            new AuthorizationCodeRefreshRequest(clientId, clientSecret, refreshToken)
            );
        return new(newToken.RefreshToken, newToken.AccessToken, newToken.CreatedAt.AddSeconds(newToken.ExpiresIn));
    }

    public async Task<UserPlaylists> GetUserPlaylistsAsync(string accessToken)
    {
        var client = new SpotifyClient(accessToken);
        var response = await client.Playlists.CurrentUsers();

        List<Playlist> playlists = [];
        foreach (var p in response.Items)
        {
            var thumbUrl = p.Images?.FirstOrDefault()?.Url ?? "";
            playlists.Add(new(p.Id, p.Name, thumbUrl));
        }
        
        return new(Provider, playlists);
    }

    public async Task<ProviderAccess> GetUserDataAsync(string accessToken)
    {
        var client = new SpotifyClient(accessToken);
        var response = await client.UserProfile.Current();

        string thumbnail = "";
        if (response.Images.FirstOrDefault() is not null)
        {
            thumbnail = response.Images.FirstOrDefault().Url;   
        }

        return new(Provider, response.DisplayName, thumbnail);
    }

    public async Task<SearchQuery> SearchForTracksAsync(string accessToken, string query)
    {
        var client = new SpotifyClient(accessToken);
        var searchRequest = new SearchRequest(SearchRequest.Types.Track, query)
        {
            Limit = 10
        };
        var response = await client.Search.Item(searchRequest);

        List<Track> tracks = [];
        foreach (var t in response.Tracks.Items)
        {
            var uploaderResult = await client.Artists.Get(t.Artists.First().Id);
            
            var id = t.Id;
            var title = t.Name;
            var thumbnailUrl = t.Album.Images.FirstOrDefault()?.Url ?? "";
            var artistsName = t.Artists.FirstOrDefault()?.Name ?? "";
            var uploaderImgUrl = uploaderResult.Images.FirstOrDefault()?.Url ?? "";
            
            tracks.Add(new(id, title, thumbnailUrl, artistsName, uploaderImgUrl));
        }

        return new(Provider, tracks);
    }

    public async Task AddSongToPlaylistAsync(string accessToken, string trackId, string playlistId)
    {
        var client = new SpotifyClient(accessToken);

        await client.Playlists.AddPlaylistItems(playlistId, new(new List<string> { $"spotify:track:{trackId}" }));
    }
}