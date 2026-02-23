using backend.DB.DAO;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly SessionService sessionService;
    private readonly AuthService authService;
    private readonly CookieService cookieService;

    public AuthController(SessionService sessionService, AuthService authService)
    {
        this.sessionService = sessionService;
        this.authService = authService;
    }

    [HttpGet("Me")]
    public async Task<ActionResult<SessionContext>> GetSessionContext()
    {
        if (!Request.Cookies.TryGetValue("Session", out var token))
            return Unauthorized();

        var user = await sessionService.GetUserBySessionToken(token);
        if (user is null)
            return Unauthorized();
        
        var refreshedSession = await sessionService.RefreshSession(token);
        cookieService.SetSession(Response, token, refreshedSession!.ExpiryDate);

        var providers = await authService.GetLinkedProviders(user.UserId);

        return Ok(new SessionContext(user.Username, providers));
    }
}