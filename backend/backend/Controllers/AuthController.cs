using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AccountService accountService;

    public AuthController(AccountService accountService)
    {
        this.accountService = accountService;
    }

    [HttpGet("Me")]
    [ProducesResponseType(typeof(SessionContext), 200)]
    public async Task<ActionResult<SessionContext>> GetSessionContext()
    {
        var user = HttpContext.GetCurrentUser();
        var providers = await accountService.GetLinkedProviders(user!.UserId);
        return Ok(new SessionContext(user.Username, providers));
    }
}