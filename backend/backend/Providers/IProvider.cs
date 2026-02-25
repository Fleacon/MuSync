using backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.Providers;

public interface IProvider
{
    public Provider Provider { get; }
    public ActionResult AuthRequest();
    public Task<OAuthResult> HandleCallbackAsync(HttpContext httpContext);

    public Task<OAuthResult> RefreshAccessTokenAsync(string refreshToken);

    public Task<UserPlaylists> GetUserPlaylistsAsync(string accessToken);

    public Task<ProviderAccess> GetUserDataAsync(string accessToken);

    public Task<SearchQuery> SearchForTracksAsync(string accessToken, string query);

    public Task AddSongToPlaylistAsync(string accessToken, string trackId, string playlistId);
    
}