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
    
    private readonly string clientId = Env.GetString("GOOGLE_CLIENTID");
    private readonly string clientSecret = Env.GetString("GOOGLE_CLIENTSECRET");
    private readonly string redirectUri = Env.GetString("GOOGLE_REDIRECTURI");
    
    private string[] scope = 
    {
        "https://www.googleapis.com/auth/youtube.readonly",
        "https://www.googleapis.com/auth/userinfo.profile",
        "https://www.googleapis.com/auth/youtube.force-ssl"
    };
    
    public ActionResult AuthRequest()
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

        return new (tokenResponse.RefreshToken, tokenResponse.AccessToken, DateTime.Now.AddSeconds((double)tokenResponse.ExpiresInSeconds));
    }
    
    public async Task<OAuthResult> RefreshAccessToken(string refreshToken)
    {
        var flow = new GoogleAuthorizationCodeFlow(new()
        {
            ClientSecrets = new() { ClientId = clientId, ClientSecret = clientSecret },
            Scopes = scope
        });

        var tokenResponse = await flow.RefreshTokenAsync("default", refreshToken, CancellationToken.None);
        return new (tokenResponse.RefreshToken, tokenResponse.AccessToken, DateTime.Now.AddSeconds((double)tokenResponse.ExpiresInSeconds));
    }

    public async Task<UserPlaylists> GetUserPlaylists(string accessToken)
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
            playlists.Add(new (item.Id, item.Snippet.Title, item.Snippet.Thumbnails.Standard.Url));
        }

        return new (Provider.YouTubeMusic, playlists);
    }

    public async Task<ProviderAccess> GetUserData(string accessToken)
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
        Console.Write(profilePicture);
        return new (Provider, channel.Snippet.Title, profilePicture);
    }

    public async Task<SearchQuery> SearchForTracks(string accessToken, string query)
    {
        var youtubeService = new YouTubeService(new()
        {
            HttpClientInitializer = GoogleCredential.FromAccessToken(accessToken)
        });

        var request = youtubeService.Search.List("snippet");
        request.Q = query;
        request.MaxResults = 10;

        var response = await request.ExecuteAsync();
        var tracks = response.Items;

        List<Track> searchQuery = [];

        foreach (var t in tracks)
        {
            var uploaderRequest = youtubeService.Channels.List("snippet");
            uploaderRequest.Id = t.Snippet.ChannelId;
            
            var uploaderResponse = await request.ExecuteAsync();
            
            var id = t.Id.ToString();
            var title = t.Snippet.Title;
            var thumbnailUrl = t.Snippet.Thumbnails.Standard.Url;
            var uploaderName = t.Snippet.ChannelTitle;
            var uploaderImgUrl = uploaderResponse.Items.First().Snippet.Thumbnails.Standard.Url;
            
            searchQuery.Add(new (id, title, thumbnailUrl, uploaderName, uploaderImgUrl));
        }

        return new(Provider, searchQuery);
    }

    public async Task AddSongToPlaylist(string accessToken, string trackId, string playlistId)
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
                    VideoId = trackId
                }
            }
        };

        var request = youtubeService.PlaylistItems.Insert(playlistItem, "snippet");
        await request.ExecuteAsync();
    }
}