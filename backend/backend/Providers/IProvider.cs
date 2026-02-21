using backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.Providers;

public interface IProvider
{
    public Provider Provider { get; }
    public ActionResult AuthRequest();
    public Task<OAuthResult> HandleCallbackAsync(HttpContext httpContext);

    public Task<OAuthResult> RefreshAccessToken(string refreshToken);

    public Task<UserPlaylists> GetUserPlaylists(string accessToken);

    public Task<ProviderAccess> GetUserData(string accessToken);

    public Task<SearchQuery> SearchForTracks(string accessToken, string query);

    public Task AddSongToPlaylist(string accessToken, string trackId, string playlistId);
    
}