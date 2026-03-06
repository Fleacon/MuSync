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
    public async Task<ActionResult<UserPlaylists>> GetUserPlaylists(Provider provider)
    {
        return await apiService.GetUserPlaylists(provider, Request);
    }

    [HttpGet("UserData/{provider}")]
    public async Task<ActionResult<ProviderAccess>> GetUserData(Provider provider)
    {
        var user = HttpContext.GetCurrentUser()!;
        if (!Request.Cookies.TryGetValue($"AccessToken_{provider.ToString()}", out var token))
        {
            var newToken = await authService.RefreshAccessToken(provider, user);
            if (newToken is null)
                return NoContent();
            cookieService.SetAccessToken(Response, provider, newToken);
            token = newToken.AccessToken;
        }

        return Ok(await apiService.GetUserData(provider, token));
    }

    [HttpGet("Search/{provider}")]
    public async Task<ActionResult<SearchQuery>> SearchForTracks(Provider provider)
    {
        var user = HttpContext.GetCurrentUser();
        if (string.IsNullOrWhiteSpace(Request.Query["q"]))
            return NoContent();
        if (!Request.Cookies.TryGetValue($"AccessToken_{provider.ToString()}", out var token))
        {
            var newToken = await authService.RefreshAccessToken(provider, user!);
            if (newToken is null)
                return NoContent();
            cookieService.SetAccessToken(Response, provider, newToken);
            token = newToken.AccessToken;
        }

        var q = Request.Query["q"];
        var query = await apiService.SearchForTracks(provider, token, q);
        
        return Ok(query);
    }

    [HttpPost("AddToPlaylists")]
    public async Task<IActionResult> AddToPlaylists([FromBody] IReadOnlyList<Selection> selections)
    {
        var user = HttpContext.GetCurrentUser();

        var tasks = selections.Select(async selection =>
        {
            if (!Request.Cookies.TryGetValue($"AccessToken_{selection.Provider}", out var token))
            {
                var newToken = await authService.RefreshAccessToken(selection.Provider, user!);
                if (newToken is null) return;
                cookieService.SetAccessToken(Response, selection.Provider, newToken);
                token = newToken.AccessToken;
            }

            await apiService.AddToPlaylist(selection.Provider, token, selection.TrackId, selection.PlaylistId);
        });

        await Task.WhenAll(tasks);
        return Ok();
    }
}