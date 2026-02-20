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
    private UsersDAO usersDao;
    private SessionService sessionService;
    private AuthService authService;

    public ProviderAuthController(UsersDAO usersDao, SessionService sessionService, AuthService authService)
    {
        this.usersDao = usersDao;
        this.sessionService = sessionService;
        this.authService = authService;
    }

    
    [HttpGet("Login/{provider}")]
    public async Task<ActionResult> Login(Provider provider)
    {
        if (!Request.Cookies.TryGetValue("Session", out var token))
            return Unauthorized();
        var user = await usersDao.GetUserByHashedSessionToken(SessionService.HashSessionToken(token));
        if (user is null)
            return Unauthorized();
        return authService.RequestAuth(provider);
    }

    [HttpGet("CallBack/{provider}")]
    public async Task<ActionResult> CallBack(Provider provider)
    {
        if (!Request.Cookies.TryGetValue("Session", out var token))
            return Unauthorized();
        var user = await usersDao.GetUserByHashedSessionToken(SessionService.HashSessionToken(token));
        if (user is null)
            return Unauthorized();
        var result = await authService.HandleCallback(provider, HttpContext);
        if (result is null)
            return BadRequest();
        //Console.WriteLine($"Refresh: {result.RefreshToken}\nAccess: {result.AccessToken}\nExpiry:{result.Expiry} ");
        await authService.CreateOAuthToken(provider, result, user.UserId);
        Response.Cookies.Append($"AccessToken_{provider}", result.AccessToken, new ()
        {
            HttpOnly = true, 
            Secure = true, 
            SameSite = SameSiteMode.Lax, 
            Path = "/"
        });
        return Redirect("https://localhost:5173/account");
    }

    [HttpGet("Refresh/{provider}")]
    public async Task<ActionResult> RefreshAccessToken(Provider provider)
    {
        if (!Request.Cookies.TryGetValue("Session", out var token))
            return Unauthorized();
        var user = await usersDao.GetUserByHashedSessionToken(SessionService.HashSessionToken(token));
        if (user is null)
            return Unauthorized();
        var newToken = await authService.RefreshAccessToken(provider, user);
        if (newToken is null)
            return BadRequest();
        Response.Cookies.Append($"AccessToken_{provider}", newToken, new ()
        {
            HttpOnly = true, 
            Secure = true, 
            SameSite = SameSiteMode.Lax, 
            Path = "/"
        });
        return Ok();
    }
}