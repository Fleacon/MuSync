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
    [ProducesResponseType(typeof(SessionContext), 200)]
    [ProducesResponseType(typeof(ApiError), 404)]
    [ProducesResponseType(typeof(ApiError),401)]
    public async Task<ActionResult<SessionContext>> TryLogin([FromBody] UserAuthData userAuthData)
    {
        var (result, user) = await accountService.ValidateCredentials(userAuthData.Username, userAuthData.Password);
        if (result == LoginResult.NotFound)
            return NotFound(new ApiError(404, "User not found"));
        if (result == LoginResult.Unauthorized)
            return Unauthorized(new ApiError(401, "Invalid username or password"));
        
        var providers = await accountService.GetLinkedProviders(user!.UserId);
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
    [ProducesResponseType(typeof(SessionContext), 200)]
    [ProducesResponseType(typeof(ApiError), 409)]
    public async Task<ActionResult<SessionContext>> TryRegister([FromBody] UserAuthData userAuthData)
    {
        var newUser = await accountService.CreateAccount(userAuthData.Username, userAuthData.Password);
        if (newUser is null)
            return Conflict(new ApiError(409, "User already exists"));

        var token = sessionService.GenerateSessionToken();
        var session = await sessionService.GenerateSession(newUser.UserId, token);
        cookieService.SetSession(Response, token, session.ExpiryDate);
        
        if (userAuthData.RememberMe)
        {
            var rawRememberToken = rememberTokenService.GenerateToken();
            var rememberToken = await rememberTokenService.GenerateRemember(newUser.UserId, rawRememberToken);
            cookieService.SetRemember(Response, rawRememberToken, rememberToken.ExpiryDate);
        }

        return Ok(new SessionContext(newUser.Username, Array.Empty<Provider>()));
    }

    [HttpPost("Logout")]
    [ProducesResponseType( 200)]
    [ProducesResponseType(typeof(ApiError), 404)]
    public async Task<ActionResult> LogOut()
    {
        var session = HttpContext.GetSessionToken();

        bool isDeleted = await sessionService.DeleteSession(session!);
        if (!isDeleted)
            return NotFound(new ApiError(404, "Session not found"));
        
        await RemoveCookies();
        return Ok();
    }
    
    [HttpPost("Disconnect/{provider}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ApiError), 404)]
    public async Task<ActionResult> DisconnectProvider(Provider provider)
    {
        var user = HttpContext.GetCurrentUser();

        bool wasDeleted = await accountService.RemoveProvider(provider, user!.UserId);
        if (!wasDeleted)
            return NotFound(new ApiError(404, $"Provider '{provider}' is not linked to this account"));
        
        cookieService.RemoveAccessToken(Response, provider);
        
        return Ok();
    }

    [HttpDelete("Delete")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ApiError), 404)]
    public async Task<ActionResult> DeleteAccount()
    {
        var user = HttpContext.GetCurrentUser();
        bool wasDeleted = await accountService.DeleteAccount(user!.UserId);
        if (!wasDeleted)
            return NotFound(new ApiError(404, "Account was not found"));
        await RemoveCookies();
        return Ok();
    }

    private async Task RemoveCookies()
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
    }
}