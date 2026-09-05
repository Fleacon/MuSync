using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using backend.Models;
using DotNetEnv;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using SoundCloudSharp.Api;
using SoundCloudSharp.Api.Authenticators;
using SoundCloudSharp.Api.Endpoints;
using SoundCloudSharp.Api.Models.Request;

namespace backend.Providers;

public class SoundCloudAPI : IProvider
{
    public Provider Provider { get; } = Provider.SoundCloud;
    
    private readonly string clientId;
    private readonly string clientSecret;
    private readonly string redirectUri;

    private readonly HttpClient httpClient;
    private readonly string apiPath = "https://api.soundcloud.com";

    private SoundCloudAPI(HttpClient httpClient, string clientId, string clientSecret, string redirectUri)
    {
        this.httpClient = httpClient;
        this.clientId = clientId;
        this.clientSecret = clientSecret;
        this.redirectUri = redirectUri;
    }

    public static bool TryCreate(HttpClient httpClient, out SoundCloudAPI? instance)
    {
        var clientId = Environment.GetEnvironmentVariable("SOUNDCLOUD_CLIENTID");
        var clientSecret = Environment.GetEnvironmentVariable("SOUNDCLOUD_CLIENTSECRET");
        var redirectUri = Environment.GetEnvironmentVariable("SOUNDCLOUD_REDIRECTURI");

        if (string.IsNullOrEmpty(clientId) ||
            string.IsNullOrEmpty(clientSecret) ||
            string.IsNullOrEmpty(redirectUri))
        {
            instance = null;
            return false;
        }

        instance = new (httpClient, clientId, clientSecret, redirectUri);
        return true;
    }

    public ActionResult AuthRequest(HttpContext httpContext)
    {
        var auth = AuthorizationCodeFlow.CreateRequest(clientId, new (redirectUri));
        
        httpContext.Response.Cookies.Append("SC_CV", auth.CodeVerifier, new()
        {
            HttpOnly  = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            MaxAge = TimeSpan.FromMinutes(10)
        });

        httpContext.Response.Cookies.Append("SC_ST", auth.State, new()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            MaxAge = TimeSpan.FromMinutes(10)
        });
        
        return new RedirectResult(auth.AuthorizationUri.ToString());
    }

    public async Task<OAuthResult> HandleCallbackAsync(HttpContext httpContext)
    {
        var code = httpContext.Request.Query["code"].ToString();
        if (string.IsNullOrEmpty(code))
            throw new ("Missing authorization code");
        
        if (!httpContext.Request.Cookies.TryGetValue("SC_CV", out var codeVerifier))
            throw new ("Missing PKCE verifier");

        if (!httpContext.Request.Cookies.TryGetValue("SC_ST", out var state))
            throw new("Missing State");
        
        var callbackUri = httpContext.Request.GetEncodedUrl();
        
        httpContext.Response.Cookies.Delete("SC_CV");
        httpContext.Response.Cookies.Delete("SC_ST");

        var request = AuthorizationCodeFlow.CreateTokenRequest(
            new(clientId, clientSecret), 
            new(callbackUri), 
            new(redirectUri), 
            codeVerifier, 
            state);

        var oAuth = new OAuthClient();
        var token = await oAuth.RequestTokenAsync(request);
        
        return new(token.RefreshToken, token.AccessToken, DateTime.Now.AddSeconds(token.ExpiresIn));
    }

    public async Task<OAuthResult> RefreshAccessTokenAsync(string refreshToken)
    {
        var oAuth = new OAuthClient();
        var newToken = await oAuth.RefreshTokenAsync(new(clientId, clientSecret), refreshToken);
        return new(newToken.RefreshToken, newToken.AccessToken, DateTime.Now.AddSeconds(newToken.ExpiresIn));
    }

    public async Task<UserPlaylists> GetUserPlaylistsAsync(string accessToken)
    {
        var client = new SoundCloudClient(accessToken);
        var firstPage = await client.Me.GetPlaylistsAsync();

        var playlists = await client
            .PaginateAllAsync(firstPage)
            .Select(sc => new Playlist(
                Id: sc.Urn,
                Title: sc.Title,
                ThumbnailUrl: sc.ArtworkUrl.ToString()))
            .ToListAsync();
        
        return new(Provider, playlists);
    }

    public async Task<ProviderAccess> GetUserDataAsync(string accessToken)
    {
        var client = new SoundCloudClient(accessToken);

        var user = await client.Me.GetAsync();

        return new(Provider, user.Username, user.AvatarUrl.ToString());
    }

    public async Task<SearchQuery> SearchForTracksAsync(string accessToken, string query)
    {
        var client = new SoundCloudClient(accessToken);

        var request = new SearchTracksRequest
        {
            Query = query,
            Page = new () { Limit = 10 }
        };

        var tracks = await client.Search.SearchTracksAsync(request);

        return new(Provider,
            tracks.Collection.Select(t => new Track(
                t.Urn.ToString(),
                t.Title,
                t.ArtworkUrl.ToString(),
                t.User.Username,
                t.User.AvatarUrl.ToString()
            )).ToArray());
    }

    public async Task AddSongToPlaylistAsync(string accessToken, string trackId, string playlistId)
    {
        var client = new SoundCloudClient(accessToken);
        
        var playlist = await client.Playlists.GetPlaylistAsync(playlistId);

        var trackUrns = playlist.Tracks
            .Select(x => x.Urn)
            .ToList();

        trackUrns.Add(trackId);
        var request = new UpdatePlaylistRequest()
        {
            Tracks = trackUrns
        };

        await client.Playlists.UpdatePlaylistAsync(playlistId, request);
    }
}