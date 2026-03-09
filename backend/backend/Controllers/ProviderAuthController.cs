using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/ProviderAuth")]
public class ProviderAuthController : ControllerBase
{
    private readonly AuthService authService;
    private readonly CookieService cookieService;

    public ProviderAuthController(AuthService authService, CookieService cookieService)
    {
        this.authService = authService;
        this.cookieService = cookieService;
    }

    [HttpGet("Login/{provider}")]
    [ProducesResponseType(302)]
    public ActionResult Login(Provider provider)
    {
        return authService.RequestAuth(provider, HttpContext);
    }

    [HttpGet("CallBack/{provider}")]
    [ProducesResponseType(302)]
    [ProducesResponseType(typeof(ApiError), 400)]
    public async Task<ActionResult> CallBack(Provider provider)
    {
        var user = HttpContext.GetCurrentUser();

        var result = await authService.HandleCallback(provider, HttpContext);
        if (result is null)
            return BadRequest(new ApiError(400, $"OAuth callback failed for provider '{provider}'"));

        await authService.CreateOAuthToken(provider, result, user!.UserId);
        cookieService.SetAccessToken(Response, provider, result);

        var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL") ?? "http://localhost";
        return Redirect($"{frontendUrl}/account");
    }

    [HttpGet("Refresh/{provider}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ApiError), 400)]
    public async Task<ActionResult> RefreshAccessToken(Provider provider)
    {
        var user = HttpContext.GetCurrentUser();

        var newToken = await authService.RefreshAccessToken(provider, user!);
        if (newToken is null)
            return BadRequest(new ApiError(400, $"Failed to refresh access token for provider '{provider}'"));

        cookieService.SetAccessToken(Response, provider, newToken);
        return Ok();
    }
}