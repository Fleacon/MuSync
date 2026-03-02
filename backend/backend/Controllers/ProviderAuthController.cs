using backend.DB.DAO;
using backend.Models;
using backend.Providers;
using backend.Services;
using DotNetEnv;
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
    public ActionResult Login(Provider provider)
    {
        return authService.RequestAuth(provider);
    }

    [HttpGet("CallBack/{provider}")]
    public async Task<ActionResult> CallBack(Provider provider)
    {
        var user = HttpContext.GetCurrentUser();

        var result = await authService.HandleCallback(provider, HttpContext);
        if (result is null)
            return BadRequest();

        await authService.CreateOAuthToken(provider, result, user.UserId);
        cookieService.SetAccessToken(Response, provider, result);

        return Redirect("https://127.0.0.1:5173/account");
    }

    [HttpGet("Refresh/{provider}")]
    public async Task<ActionResult> RefreshAccessToken(Provider provider)
    {
        var user = HttpContext.GetCurrentUser();

        var newToken = await authService.RefreshAccessToken(provider, user);
        if (newToken is null)
            return BadRequest();

        cookieService.SetAccessToken(Response, provider, newToken);
        return Ok();
    }
}