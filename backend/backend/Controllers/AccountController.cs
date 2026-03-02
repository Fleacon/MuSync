using backend.DB.DAO;
using backend.Filter;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly AccountService accountService;
    private readonly SessionService sessionService;
    private readonly CookieService cookieService;
    private readonly RememberTokenService rememberTokenService;

    public AccountController(AccountService accountService, SessionService sessionService, CookieService cookieService, RememberTokenService rememberTokenService)
    {
        this.accountService = accountService;
        this.sessionService = sessionService;
        this.cookieService = cookieService;
        this.rememberTokenService = rememberTokenService;
    }

    [HttpPost("Login")]
    [SkipSessionAuth]
    public async Task<ActionResult<SessionContext>> TryLogin([FromBody] UserAuthData userAuthData)
    {
        var (result, user) = await accountService.ValidateCredentials(userAuthData.Username, userAuthData.Password);
        Console.WriteLine($"Connects: {user.Username}");
        if (result == LoginResult.NOTFOUND)
            return NotFound();
        if (result == LoginResult.UNAUTHORIZED)
            return Unauthorized();
        
        var providers = await accountService.GetLinkedProviders(user.UserId);
        var token = sessionService.GenerateSessionToken();
        var session = await sessionService.GenerateSession(user.UserId, token);
        cookieService.SetSession(Response, token, session.ExpiryDate);

        if (userAuthData.RememberMe)
        {
            var rawRememberToken = rememberTokenService.GenerateToken();
            var rememberToken = await rememberTokenService.GenerateRemember(user.UserId, rawRememberToken);
            cookieService.SetRemember(Response, rawRememberToken, rememberToken.ExpiryDate);
        }

        return Ok(new SessionContext(user.Username, providers));
    }

    [HttpPost("Register")]
    [SkipSessionAuth]
    public async Task<ActionResult<SessionContext>> TryRegister([FromBody] UserAuthData userAuthData)
    {
        var newUser = await accountService.CreateAccount(userAuthData.Username, userAuthData.Password);
        if (newUser is null)
            return Conflict();

        var token = sessionService.GenerateSessionToken();
        var session = await sessionService.GenerateSession(newUser.UserId, token);
        cookieService.SetSession(Response, token, session.ExpiryDate);
        
        if (userAuthData.RememberMe)
        {
            var rawRememberToken = rememberTokenService.GenerateToken();
            var rememberToken = await rememberTokenService.GenerateRemember(newUser.UserId, rawRememberToken);
            cookieService.SetRemember(Response, rawRememberToken, rememberToken.ExpiryDate);
        }

        return Ok(new SessionContext(newUser.Username, null));
    }

    [HttpPost("Logout")]
    public async Task<IActionResult> LogOut()
    {
        if (!Request.Cookies.TryGetValue("Session", out var sToken))
            return NoContent();

        bool isDeleted = await sessionService.DeleteSession(sToken);
        if (isDeleted)
        {
            Response.Cookies.Delete("Session");
            
            if (Request.Cookies.TryGetValue("Remember", out var rememberToken))
            {
                await rememberTokenService.DeleteSession(rememberToken);
                Response.Cookies.Delete("Remember");
            }
            
            foreach (var cookie in Request.Cookies.Keys)
            {
                if (cookie.StartsWith("AccessToken_"))
                {
                    Response.Cookies.Delete(cookie);
                }
            }
            
            return Ok();
        }

        return NotFound();
    }
    
    [HttpPost("Disconnect/{provider}")]
    public async Task<ActionResult> DisconnectProvider(Provider provider)
    {
        if (!Request.Cookies.TryGetValue("Session", out var token))
            return Unauthorized();

        var user = await sessionService.GetUserBySessionToken(token);
        if (user is null)
            return Unauthorized();

        bool wasDeleted = await accountService.RemoveProvider(provider, user.UserId);
        if (!wasDeleted)
            return NotFound();
        
        cookieService.RemoveAccessToken(Response, provider);
        
        return Ok();
    }
}