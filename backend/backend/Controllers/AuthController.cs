using backend.DB.DAO;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private SessionsDAO sessionsDao;
    private UsersDAO usersDao;
    private OAuthTokensDAO authTokensDao;
    private SessionService sessionService;

    public AuthController(SessionsDAO sessionsDao, UsersDAO usersDao, OAuthTokensDAO authTokensDao, SessionService sessionService)
    {
        this.sessionsDao = sessionsDao;
        this.usersDao = usersDao;
        this.authTokensDao = authTokensDao;
        this.sessionService = sessionService;
    }
    
    [HttpGet("Me")]
    public async Task<ActionResult<SessionContext>> GetSessionContext()
    {
        if (!Request.Cookies.TryGetValue("Session", out var token))
            return Unauthorized();
        var user = await usersDao.GetUserByHashedSessionToken(sessionService.HashSessionToken(token));
        if (user is null)
            return Unauthorized();
        var providers = await authTokensDao.GetOAuthTokenByUserId(user.UserId);
        var providersList = providers
            .Select(p => p.Provider)
            .Distinct()
            .ToList();

        var context = new SessionContext(user.Username, providersList);
        return Ok(context);
    }
}