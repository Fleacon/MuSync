using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PreferencesController : ControllerBase
{
    private readonly PreferencesService preferencesService;

    public PreferencesController(PreferencesService preferencesService)
    {
        this.preferencesService = preferencesService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyDictionary<string, string>>> GetAll()
    {
        var user = HttpContext.GetCurrentUser()!;
        var preferences = await preferencesService.GetAllPreferences(user.UserId);
        return Ok(preferences);
    }

    [HttpGet("{key}")]
    public async Task<ActionResult<string>> Get(string key)
    {
        var user = HttpContext.GetCurrentUser()!;
        var value = await preferencesService.GetPreference(user.UserId, key);
        if (value is null)
            return NotFound();
        return Ok(value);
    }

    [HttpPut("{key}")]
    public async Task<IActionResult> Set(string key, [FromBody] string value)
    {
        var user = HttpContext.GetCurrentUser()!;
        await preferencesService.SetPreference(user.UserId, key, value);
        return Ok();
    }

    [HttpDelete("{key}")]
    public async Task<IActionResult> Delete(string key)
    {
        var user = HttpContext.GetCurrentUser()!;
        var wasDeleted = await preferencesService.DeletePreference(user.UserId, key);
        if (!wasDeleted)
            return NotFound();
        return Ok();
    }
}