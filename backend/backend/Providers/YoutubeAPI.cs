using backend.Models;
using DotNetEnv;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.AspNetCore.Mvc;
using Playlist = backend.Models.Playlist;

namespace backend.Providers;

public class YoutubeAPI : IProvider
{
    public Provider Provider => Provider.YouTubeMusic;
    
    private readonly string clientId;
    private readonly string clientSecret;
    private readonly string redirectUri;
    
    private string[] scope = 
    {
        "https://www.googleapis.com/auth/youtube.readonly",
        "https://www.googleapis.com/auth/userinfo.profile",
        "https://www.googleapis.com/auth/youtube.force-ssl"
    };
    
    private YoutubeAPI(string clientId, string clientSecret, string redirectUri)
    {
        this.clientId = clientId;
        this.clientSecret = clientSecret;
        this.redirectUri = redirectUri;
    }

    public static bool TryCreate(out YoutubeAPI? instance)
    {
        var clientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENTID");
        var clientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENTSECRET");
        var redirectUri = Environment.GetEnvironmentVariable("GOOGLE_REDIRECTURI");

        if (string.IsNullOrEmpty(clientId) ||
            string.IsNullOrEmpty(clientSecret) ||
            string.IsNullOrEmpty(redirectUri))
        {
            instance = null;
            return false;
        }

        instance = new(clientId, clientSecret, redirectUri);
        return true;
    }
    
    public ActionResult AuthRequest(HttpContext httpContext)
    {
        var flow = new GoogleAuthorizationCodeFlow(new()
        {
            ClientSecrets = new() { ClientId = clientId, ClientSecret = clientSecret },
            Scopes = scope,
            Prompt = "consent",
        });

        var request = flow.CreateAuthorizationCodeRequest(redirectUri); 
        return new RedirectResult(request.Build().ToString());
    }

    public async Task<OAuthResult> HandleCallbackAsync(HttpContext httpContext)
    {
        string code = httpContext.Request.Query["code"].ToString();
        
        var flow = new GoogleAuthorizationCodeFlow(new()
        {
            ClientSecrets = new() { ClientId = clientId, ClientSecret = clientSecret },
            Scopes = scope
        });

        var tokenResponse = await flow.ExchangeCodeForTokenAsync("default", code, redirectUri, CancellationToken.None);

        return new (tokenResponse.RefreshToken, tokenResponse.AccessToken, DateTime.Now.AddSeconds((double)tokenResponse.ExpiresInSeconds!));
    }
    
    public async Task<OAuthResult> RefreshAccessTokenAsync(string refreshToken)
    {
        var flow = new GoogleAuthorizationCodeFlow(new()
        {
            ClientSecrets = new() { ClientId = clientId, ClientSecret = clientSecret },
            Scopes = scope
        });

        var tokenResponse = await flow.RefreshTokenAsync("default", refreshToken, CancellationToken.None);
        return new (tokenResponse.RefreshToken, tokenResponse.AccessToken, DateTime.Now.AddSeconds((double)tokenResponse.ExpiresInSeconds!));
    }

    public async Task<UserPlaylists> GetUserPlaylistsAsync(string accessToken)
    {
        var youtubeService = new YouTubeService(new()
        {
            HttpClientInitializer = GoogleCredential.FromAccessToken(accessToken)
        });

        var request = youtubeService.Playlists.List("snippet");
        request.Mine = true;
        var response = await request.ExecuteAsync();

        List<Playlist> playlists = [];
        foreach (var item in response.Items)
        {
            var thumbnail = item.Snippet.Thumbnails.Standard?.Url
                            ?? item.Snippet.Thumbnails.High?.Url
                            ?? item.Snippet.Thumbnails.Medium?.Url
                            ?? item.Snippet.Thumbnails.Default__?.Url
                            ?? "";
            playlists.Add(new (item.Id, item.Snippet.Title, thumbnail));
        }

        return new (Provider.YouTubeMusic, playlists);
    }

    public async Task<ProviderAccess> GetUserDataAsync(string accessToken)
    {
        var youtubeService = new YouTubeService(new()
        {
            HttpClientInitializer = GoogleCredential.FromAccessToken(accessToken)
        });

        var request = youtubeService.Channels.List("snippet");
        request.Mine = true;

        var response = await request.ExecuteAsync();
        var channel = response.Items.FirstOrDefault();
        
        if (channel == null)
            return null;
        
        var thumbnails = channel.Snippet.Thumbnails;

        var profilePicture =
            thumbnails?.High?.Url ??
            thumbnails?.Medium?.Url ??
            thumbnails?.Default__?.Url ??
            "";

        return new (Provider, channel.Snippet.Title, profilePicture);
    }

    public async Task<SearchQuery> SearchForTracksAsync(string accessToken, string query)
    {
        var youtubeService = new YouTubeService(new()
        {
            HttpClientInitializer = GoogleCredential.FromAccessToken(accessToken)
        });

        var request = youtubeService.Search.List("snippet");
        request.Q = query;
        request.MaxResults = 10;
        request.Type = "video";
        request.VideoCategoryId = "10"; // Music

        var response = await request.ExecuteAsync();
        var tracks = response.Items;

        List<Track> searchQuery = [];

        foreach (var t in tracks)
        {
            var uploaderRequest = youtubeService.Channels.List("snippet");
            uploaderRequest.Id = t.Snippet.ChannelId;
            
            var uploaderResponse = await uploaderRequest.ExecuteAsync();
            
            var id = t.Id.VideoId;
            var title = t.Snippet.Title;
            var thumbnailUrl = t.Snippet.Thumbnails.Standard?.Url
                               ?? t.Snippet.Thumbnails.High?.Url
                               ?? t.Snippet.Thumbnails.Medium?.Url
                               ?? t.Snippet.Thumbnails.Default__?.Url
                               ?? "";
            var uploaderName = t.Snippet.ChannelTitle;
            var uploaderImgUrl = uploaderResponse.Items?.FirstOrDefault()?.Snippet.Thumbnails.Standard?.Url
                                 ?? uploaderResponse.Items?.FirstOrDefault()?.Snippet.Thumbnails.Default__?.Url
                                 ?? "";
            
            searchQuery.Add(new (id, title, thumbnailUrl, uploaderName, uploaderImgUrl));
        }
        
        return new(Provider, searchQuery);
    }

    public async Task AddSongToPlaylistAsync(string accessToken, string trackId, string playlistId)
    {
        var youtubeService = new YouTubeService(new()
        {
            HttpClientInitializer = GoogleCredential.FromAccessToken(accessToken)
        });

        var playlistItem = new PlaylistItem
        {
            Snippet = new ()
            {
                PlaylistId = playlistId,
                ResourceId = new ()
                {
                    Kind = "youtube#video",
                    VideoId = trackId
                }
            }
        };

        var request = youtubeService.PlaylistItems.Insert(playlistItem, "snippet");
        await request.ExecuteAsync();
    }
}