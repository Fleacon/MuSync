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
    private SessionsDAO sessionsDao;
    private OAuthTokensDAO authTokensDao;
    private SessionService sessionService;
    private TokenService tokenService;
    private AuthService authService;

    public ProviderAuthController(UsersDAO usersDao, SessionsDAO sessionsDao, OAuthTokensDAO oAuthDao)
    {
        this.usersDao = usersDao;
        this.sessionsDao = sessionsDao;
        authTokensDao = oAuthDao;
        sessionService = new (sessionsDao);
        tokenService = new(oAuthDao);
        authService = new(new IProvider[]
        {
            new Providers.SpotifyAPI(),
        });
    }

    
    [HttpGet("Login/{provider}")]
    public async Task<ActionResult> Login(Provider provider)
    {
        if (!Request.Cookies.TryGetValue("Sessions", out var token))
            return Unauthorized();
        var user = await usersDao.GetUserByHashedSessionToken(sessionService.HashSessionToken(token));
        if (user is null)
            return Unauthorized();
        return authService.RequestAuth(provider, HttpContext);
    }

    [HttpGet("CallBack/{provider}")]
    public async Task<ActionResult> CallBack(Provider provider)
    {
        if (!Request.Cookies.TryGetValue("Sessions", out var token))
            return Unauthorized();
        var user = await usersDao.GetUserByHashedSessionToken(sessionService.HashSessionToken(token));
        if (user is null)
            return Unauthorized();
        var result = await authService.HandleCallback(provider, HttpContext);
        if (result is null)
            return BadRequest();
        await authTokensDao.CreateOAuthToken(new(0, provider, result.RefreshToken, user.UserId));
        Response.Cookies.Append($"AccessToken_{provider.ToString()}", result.AccessToken);
        return Ok();
    }
}