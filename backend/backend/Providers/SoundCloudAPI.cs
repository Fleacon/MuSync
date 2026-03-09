using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using backend.Models;
using DotNetEnv;
using Microsoft.AspNetCore.Mvc;

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
        var codeVerifier = GenerateCodeVerifier();
        var codeChallenge = GenerateCodeChallenge(codeVerifier);
        var state = GenerateCodeVerifier();
        
        httpContext.Response.Cookies.Append("SC_CV", codeVerifier, new()
        {
            HttpOnly  = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            MaxAge = TimeSpan.FromMinutes(10)
        });
        
        var uri = "https://secure.soundcloud.com/authorize" +
                  $"?client_id={clientId}" +
                  $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                  "&response_type=code" +
                  $"&code_challenge={codeChallenge}" +
                  "&code_challenge_method=S256" +
                  $"&state={state}";
        
        return new RedirectResult(uri);
    }

    public async Task<OAuthResult> HandleCallbackAsync(HttpContext httpContext)
    {
        var code = httpContext.Request.Query["code"].ToString();
        if (string.IsNullOrEmpty(code))
            throw new ("Missing authorization code");
        
        if (!httpContext.Request.Cookies.TryGetValue("SC_CV", out var codeVerifier))
            throw new ("Missing PKCE verifier");
        
        httpContext.Response.Cookies.Delete("SC_CV");

        var response = await httpClient.PostAsync("https://secure.soundcloud.com/oauth/token",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code",
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["redirect_uri"] = redirectUri,
                ["code_verifier"] = codeVerifier, 
                ["code"] = code,
            }));
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadFromJsonAsync<SoundCloudTokenResponse>();

        return new(json!.RefreshToken, json.AccessToken, DateTime.Now.AddSeconds(json.ExpiresIn));
    }

    public async Task<OAuthResult> RefreshAccessTokenAsync(string refreshToken)
    {
        var response = await httpClient.PostAsync("https://secure.soundcloud.com/oauth/token",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["refresh_token"] = refreshToken
            }));
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"SoundCloud refresh failed: {response.StatusCode} - {error}");
            throw new HttpRequestException($"Refresh failed: {error}");
        }
        var json = await response.Content.ReadFromJsonAsync<SoundCloudTokenResponse>();

        return new(json!.RefreshToken, json.AccessToken, DateTime.Now.AddSeconds(json.ExpiresIn));
    }

    public async Task<UserPlaylists> GetUserPlaylistsAsync(string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{apiPath}/me/playlists");
        request.Headers.Authorization = new("OAuth", accessToken);

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadFromJsonAsync<List<SoundCloudPlaylist>>();
        
        List<Playlist> playlists = [];
        playlists.AddRange(json!.Select(p => new Playlist(p.Id, p.Title, p.ArtworkUrl ?? "")));

        return new(Provider, playlists);
    }

    public async Task<ProviderAccess> GetUserDataAsync(string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{apiPath}/me");
        request.Headers.Authorization = new("OAuth", accessToken);

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadFromJsonAsync<SoundCloudUser>();

        return new(Provider, json!.Username, json.Avatar);
    }

    public async Task<SearchQuery> SearchForTracksAsync(string accessToken, string query)
    {
        var request = new HttpRequestMessage(HttpMethod.Get,
            $"{apiPath}/tracks?q={Uri.EscapeDataString(query)}&limit=10");
        request.Headers.Authorization = new ("OAuth", accessToken);

        var searchResponse = await httpClient.SendAsync(request);
        searchResponse.EnsureSuccessStatusCode();
        var tracks = await searchResponse.Content.ReadFromJsonAsync<List<SoundCloudTrack>>();

        return new(Provider,
            tracks!.Select(t => new Track(
                t.Id.ToString(),
                t.Title,
                t.ArtworkUrl ?? "",
                t.User.Username,
                t.User.Avatar
            )).ToArray());
    }

    public async Task AddSongToPlaylistAsync(string accessToken, string trackId, string playlistId)
    {
        var getRequest = new HttpRequestMessage(HttpMethod.Get,
            $"{apiPath}/playlists/{playlistId}?show_tracks=true");
        getRequest.Headers.Authorization = new("OAuth", accessToken);

        var getResponse = await httpClient.SendAsync(getRequest);
        getResponse.EnsureSuccessStatusCode();
        
        var playlist = await getResponse.Content.ReadFromJsonAsync<SoundCloudPlaylist>();
        
        var updatedTracks = playlist!.Tracks
            .Select(t => new { id = t.Id })
            .Append(new { id = int.Parse(trackId) })
            .ToList();

        var putRequest = new HttpRequestMessage(HttpMethod.Put, $"{apiPath}/playlists/{playlistId}");
        putRequest.Headers.Authorization = new("OAuth", accessToken);
        putRequest.Content = JsonContent.Create(new
        {
            playlist = new { tracks = updatedTracks }
        });
        
        var putResponse = await httpClient.SendAsync(putRequest);
        putResponse.EnsureSuccessStatusCode();
    }
    
    private static string GenerateCodeVerifier()
    {
        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Base64UrlEncode(bytes);
    }

    private static string GenerateCodeChallenge(string codeVerifier)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(codeVerifier));
        return Base64UrlEncode(hash);
    }

    private static string Base64UrlEncode(byte[] bytes) =>
        Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    
    // --- Models --- 
    
    record SoundCloudTokenResponse(
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("refresh_token")] string RefreshToken,
        [property: JsonPropertyName("expires_in")] int ExpiresIn
    );

    private record SoundCloudTrack(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("user")] SoundCloudUser User,
        [property: JsonPropertyName("artwork_url")] string? ArtworkUrl
    );

    private record SoundCloudUser(
        [property: JsonPropertyName("username")] string Username,
        [property: JsonPropertyName("avatar_url")] string Avatar
    );

    private record SoundCloudPlaylist(
        [property: JsonPropertyName("urn")] string Id,
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("artwork_url")] string? ArtworkUrl,
        [property: JsonPropertyName("tracks")] List<SoundCloudTrack> Tracks
    );
}