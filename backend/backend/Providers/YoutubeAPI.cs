using backend.Models;
using DotNetEnv;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Mvc;
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
        "https://www.googleapis.com/auth/userinfo.profile"
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

        return new (tokenResponse.RefreshToken, tokenResponse.AccessToken, DateTime.MaxValue);
    }
    
    public async Task<string> RefreshAccessToken(string refreshToken)
    {
        var flow = new GoogleAuthorizationCodeFlow(new()
        {
            ClientSecrets = new() { ClientId = clientId, ClientSecret = clientSecret },
            Scopes = scope
        });

        var tokenResponse = await flow.RefreshTokenAsync("default", refreshToken, CancellationToken.None);
        return tokenResponse.AccessToken;
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

        return new (Provider, channel.Snippet.Title);
    }

    public async Task<bool> IsTokenValid(string accessToken)
    {
        try
        {
            var service = new YouTubeService(new()
            {
                HttpClientInitializer = GoogleCredential.FromAccessToken(accessToken)
            });
            var request = service.Channels.List("id");
            request.Mine = true;
            await request.ExecuteAsync();
            Console.WriteLine(request);
            return true;
        }
        catch
        {
            return false;
        }
    }
}