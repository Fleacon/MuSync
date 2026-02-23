using backend.DB.DAO;
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
    private readonly AuthService authService;
    private readonly CookieService cookieService;

    public AccountController(AccountService accountService, SessionService sessionService, AuthService authService, CookieService cookieService)
    {
        this.accountService = accountService;
        this.sessionService = sessionService;
        this.authService = authService;
        this.cookieService = cookieService;
    }

    [HttpPost("Login")]
    public async Task<ActionResult<SessionContext>> TryLogin([FromBody] UserAuthData userAuthData)
    {
        User? user;
        try
        {
            user = await accountService.ValidateCredentials(userAuthData.Username, userAuthData.Password);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }

        if (user is null)
            return NotFound();

        var providers = await authService.GetLinkedProviders(user.UserId);

        var token = sessionService.GenerateSessionToken();
        var session = await sessionService.GenerateSession(user.UserId, token);
        cookieService.SetSession(Response, token, session.ExpiryDate);

        return Ok(new SessionContext(user.Username, providers));
    }

    [HttpPost("Register")]
    public async Task<ActionResult<SessionContext>> TryRegister([FromBody] UserAuthData userAuthData)
    {
        var newUser = await accountService.CreateAccount(userAuthData.Username, userAuthData.Password);
        if (newUser is null)
            return Conflict();

        var token = sessionService.GenerateSessionToken();
        var session = await sessionService.GenerateSession(newUser.UserId, token);
        cookieService.SetSession(Response, token, session.ExpiryDate);

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
            return Ok();
        }

        return NotFound();
    }
}