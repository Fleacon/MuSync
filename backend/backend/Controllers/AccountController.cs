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
    private SessionManager sessionManager;
    private TokenManager tokenManager;

    public AccountController(UsersDAO usersDao, SessionsDAO sessionsDao, OAuthTokensDAO oAuthDao)
    {
        this.usersDao = usersDao;
        this.sessionsDao = sessionsDao;
        sessionManager = new (this.sessionsDao);
        tokenManager = new(oAuthDao);
    }

    [HttpPost("LoginData")]
    public async Task<ActionResult<SessionContext>> TryLogin([FromBody] UserAuthData userAuthData)
    {
        if (user == null)
        {
            return Unauthorized();
        }
        var user = await usersDao.GetUserByUsername(userAuthData.Username);

        string storedPw = user.PasswordHash;
        if (!PasswordService.VerifyPassword(storedPw, userAuthData.Password))
            return Unauthorized();
        int userId = user.UserId; 
        string sessionToken = sessionManager.GenerateSessionToken(); 
        var s = await sessionManager.GenerateSession(userId, sessionToken); 
        Response.Cookies.Append("Session", sessionToken, new () 
        { 
            HttpOnly = true, 
            Secure = true, 
            SameSite = SameSiteMode.Lax, 
            Expires = s.ExpiryDate,
            Path = "/"
        }); 
        string username = user.Username; 
        var providerAccesses = await tokenManager.GenerateProviderAccess(user); 
        return Ok(new SessionContext(username, providerAccesses));
    }
}