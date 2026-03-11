using backend.Models;
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
    [ProducesResponseType(typeof(IReadOnlyDictionary<string, string>), 200)]
    public async Task<ActionResult<IReadOnlyDictionary<string, string>>> GetAll()
    {
        var user = HttpContext.GetCurrentUser()!;
        var preferences = await preferencesService.GetAllPreferences(user.UserId);
        return Ok(preferences);
    }

    [HttpGet("{key}")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(typeof(ApiError), 404)]
    public async Task<ActionResult<string>> Get(string key)
    {
        var user = HttpContext.GetCurrentUser()!;
        var value = await preferencesService.GetPreference(user.UserId, key);
        if (value is null)
            return NotFound(new ApiError(404, $"Preference '{key}' not found"));
        return Ok(value);
    }

    [HttpPut("{key}")]
    [ProducesResponseType(200)]
    public async Task<ActionResult> Set(string key, [FromBody] string value)
    {
        var user = HttpContext.GetCurrentUser()!;
        await preferencesService.SetPreference(user.UserId, key, value);
        return Ok();
    }

    [HttpDelete("{key}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ApiError), 404)]
    public async Task<ActionResult> Delete(string key)
    {
        var user = HttpContext.GetCurrentUser()!;
        var wasDeleted = await preferencesService.DeletePreference(user.UserId, key);
        if (!wasDeleted)
            return NotFound(new ApiError(404, $"Preference '{key}' not found"));
        return Ok();
    }
}