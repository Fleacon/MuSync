using backend.DB.DAO;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private UsersDAO usersDao;
    private SessionsDAO sessionsDao;
    private OAuthTokensDAO authTokensDao;
    private SessionService sessionService;
    private CookieService cookieService;

    public AccountController(UsersDAO usersDao, SessionsDAO sessionsDao, OAuthTokensDAO oAuthDao, SessionService sessionService, CookieService cookieService)
    {
        this.usersDao = usersDao;
        this.sessionsDao = sessionsDao;
        this.authTokensDao = oAuthDao;
        this.sessionService = sessionService;
        this.cookieService = cookieService;
    }

    [HttpPost("Login")] // TODO: Implement Remember Me
    public async Task<ActionResult<SessionContext>> TryLogin([FromBody] UserAuthData userAuthData)
    {
        var user = await usersDao.GetUserByUsername(userAuthData.Username);
        if (user is null)
            return NotFound();

        string storedPw = user.PasswordHash;
        if (!PasswordService.VerifyPassword(storedPw, userAuthData.Password))
            return Unauthorized();
        
        var providers = await authTokensDao.GetOAuthTokenByUserId(user.UserId);
        var providersList = providers
            .Select(p => p.Provider)
            .Distinct()
            .ToList();

        string token = sessionService.GenerateSessionToken();
        var session = await sessionService.GenerateSession(user.UserId, token);
        cookieService.SetSession(Response, token, session.ExpiryDate);
        
        return Ok(new SessionContext(user.Username, providersList));
    }

    [HttpPost("Register")]
    public async Task<ActionResult<SessionContext>> TryRegister([FromBody] UserAuthData userAuthData)
    {
        var user = await usersDao.GetUserByUsername(userAuthData.Username);
        if (user is not null)
            return Conflict();

        string username = userAuthData.Username;
        string hashedPw = PasswordService.HashPassword(userAuthData.Password);

        var newUser = await usersDao.CreateUser(new(0, username, hashedPw));

        string token = sessionService.GenerateSessionToken();
        var session = await sessionService.GenerateSession(newUser.UserId, token);
        cookieService.SetSession(Response, token, session.ExpiryDate);

        return Ok(new SessionContext(newUser.Username, null));
    }

    [HttpPost("Logout")]
    public async Task<IActionResult> LogOut()
    {
        if (!Request.Cookies.TryGetValue("Session", out var sToken))
        {
            return NoContent();
        }
        bool isDeleted = await sessionService.DeleteSession(sToken);
        if (isDeleted)
        {
            Response.Cookies.Delete("Session");
            return Ok();
        }
        return NotFound();   
    }
}