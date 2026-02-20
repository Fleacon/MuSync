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

    public ProviderController(ProviderApiService apiService, AuthService authService)
    {
        this.apiService = apiService;
        this.authService = authService;
    }
    
    [HttpGet("Get/Playlists/{provider}")]
    public async Task<ActionResult<UserPlaylists>> GetUserPlaylists(Provider provider)
    {
        if (!Request.Cookies.TryGetValue("Session", out var token))
            return Unauthorized();
        return await apiService.GetUserPlaylists(provider, HttpContext);
    }
    
    [HttpGet("LinkedProviders")]
    public async Task<ActionResult<IReadOnlyList<ProviderAccess>>> GetLinkedProviders()
    {
        if (!Request.Cookies.TryGetValue("Session", out var sToken))
            return NoContent();

        var providerAccesses = await apiService.GetConnectedUserData(HttpContext);
        return Ok(providerAccesses);
    }
}