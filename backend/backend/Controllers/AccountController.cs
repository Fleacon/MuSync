using backend.DB.DAO;
using backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private UsersDAO usersDao;
    private SessionsDAO sessionsDao;
    private OAuthTokensDAO authTokensDao;
    private SessionManager sessionManager;
    private TokenManager tokenManager;

    public AccountController(UsersDAO usersDao, SessionsDAO sessionsDao, OAuthTokensDAO oAuthDao)
    {
        this.usersDao = usersDao;
        this.sessionsDao = sessionsDao;
        authTokensDao = oAuthDao;
        sessionManager = new (this.sessionsDao);
        tokenManager = new(oAuthDao);
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

        await sessionManager.GenerateSession(Response, user.UserId);
        await tokenManager.GenerateProviderAccess(Response, user);
        
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

        await sessionManager.GenerateSession(Response, newUser.UserId);

        return Ok(new SessionContext(newUser.Username, null));
    }

    [HttpPost("Logout")]
    public async Task<IActionResult> LogOut()
    {
        if (!Request.Cookies.TryGetValue("Session", out var sToken))
        {
            return NoContent();
        }
        bool isDeleted = await sessionManager.DeleteSession(sToken);
        if (isDeleted)
        {
            Response.Cookies.Delete("Session");
            return Ok();
        }
        return NotFound();   
    }
}