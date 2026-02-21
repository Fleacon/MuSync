using backend.DB.DAO;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/ProviderController")]
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
        if (!Request.Cookies.TryGetValue("Session", out var token))
            return Unauthorized();
        return await apiService.GetUserPlaylists(provider, HttpContext);
    }

    [HttpGet("UserData/{provider}")]
    public async Task<ActionResult<ProviderAccess>> GetUserData(Provider provider)
    {
        if (!Request.Cookies.TryGetValue("Session", out var sToken))
            return Unauthorized();
        if (!Request.Cookies.TryGetValue($"AccessToken_{provider.ToString()}", out var token))
        {
            var newToken = await authService.RefreshAccessToken(provider, sToken);
            if (newToken is null)
                return NoContent();
            cookieService.SetAccessToken(Response, provider, newToken);
            token = newToken.AccessToken;
        }

        return Ok(await apiService.GetUserData(provider, token));
    }
}