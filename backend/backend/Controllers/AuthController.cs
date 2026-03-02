using backend.DB.DAO;
using backend.Filter;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly SessionService sessionService;
    private readonly CookieService cookieService;
    private readonly AccountService accountService;

    public AuthController(SessionService sessionService, CookieService cookieService, AccountService accountService)
    {
        this.sessionService = sessionService;
        this.cookieService = cookieService;
        this.accountService = accountService;
    }

    [HttpGet("Me")]
    public async Task<ActionResult<SessionContext>> GetSessionContext()
    {
        var user = HttpContext.GetCurrentUser()!;
        var providers = await accountService.GetLinkedProviders(user.UserId);
        return Ok(new SessionContext(user.Username, providers));
    }
}