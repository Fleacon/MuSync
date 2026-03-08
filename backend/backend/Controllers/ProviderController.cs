using System.Collections.Concurrent;
using backend.DB.DAO;
using backend.Filter;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/Provider")]
public class ProviderController : ControllerBase
{
    private readonly ProviderApiService apiService;
    private readonly AuthService authService;
    private readonly CookieService cookieService;

    public ProviderController(ProviderApiService apiService, AuthService authService, CookieService cookieService)
    {
        this.apiService = apiService;
        this.authService = authService;
        this.cookieService = cookieService;
    }
    
    [HttpGet("Get/Playlists/{provider}")]
    [ProducesResponseType(typeof(UserPlaylists), 200)]
    public async Task<ActionResult<UserPlaylists>> GetUserPlaylists(Provider provider)
    {
        return await apiService.GetUserPlaylists(provider, Request);
    }

    [HttpGet("UserData/{provider}")]
    [ProducesResponseType(typeof(ProviderAccess), 200)]
    [ProducesResponseType(typeof(ApiError), 401)]
    public async Task<ActionResult<ProviderAccess>> GetUserData(Provider provider)
    {
        var user = HttpContext.GetCurrentUser()!;
        var token = await GetOrRefreshAccessToken(provider, user);
        if (token is null)
            return Unauthorized(new ApiError(401, $"Access token for '{provider}' could not be refreshed. Please reconnect."));

        return Ok(await apiService.GetUserData(provider, token));
    }

    [HttpGet("Search/{provider}")]
    [ProducesResponseType(typeof(SearchQuery), 200)]
    [ProducesResponseType(typeof(ApiError), 400)]
    [ProducesResponseType(typeof(ApiError), 401)]
    public async Task<ActionResult<SearchQuery>> SearchForTracks(Provider provider)
    {
        if (string.IsNullOrWhiteSpace(Request.Query["q"]))
            return BadRequest(new ApiError(400, "Query parameter 'q' is required"));

        var user = HttpContext.GetCurrentUser()!;
        var token = await GetOrRefreshAccessToken(provider, user);
        if (token is null)
            return Unauthorized(new ApiError(401, $"Access token for '{provider}' could not be refreshed. Please reconnect."));

        return Ok(await apiService.SearchForTracks(provider, token, Request.Query["q"]));
    }

    [HttpPost("AddToPlaylists")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ApiError), 400)] 
    public async Task<IActionResult> AddToPlaylists([FromBody] IReadOnlyList<Selection> selections)
    {
        if (selections.Count == 0)
            return BadRequest(new ApiError(400, "No selections provided"));
        
        var user = HttpContext.GetCurrentUser();
        var failed = new ConcurrentBag<Selection>();

        await Task.WhenAll(selections.Select(async selection =>
        {
            var token = await GetOrRefreshAccessToken(selection.Provider, user!);
            if (token is null)
            {
                failed.Add(selection);
                return;
            }
            await apiService.AddToPlaylist(selection.Provider, token, selection.TrackId, selection.PlaylistId);
        }));

        if (!failed.IsEmpty)
            return Ok(new
            {
                succeeded = selections.Except(failed),
                failed
            });

        return Ok();
    }
    
    private async Task<string?> GetOrRefreshAccessToken(Provider provider, User user)
    {
        if (Request.Cookies.TryGetValue($"AccessToken_{provider}", out var token))
            return token;

        var newToken = await authService.RefreshAccessToken(provider, user);
        if (newToken is null)
            return null;

        cookieService.SetAccessToken(Response, provider, newToken);
        return newToken.AccessToken;
    }
}