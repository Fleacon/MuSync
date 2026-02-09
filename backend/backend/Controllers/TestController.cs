using backend.DB;
using backend.DB.DAO;
using backend.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly UsersDAO uDao;

    public TestController(UsersDAO userDao)
    {
        uDao = userDao;
    }

    [HttpGet]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await uDao.GetUserById(id);
        if (user is null) return NotFound();
        return Ok(user);
    }

    [HttpPost("CreateUser")]
    public async Task<ActionResult<User>> CreateUser([FromBody] User u)
    {
        var newUser = await uDao.CreateUser(u);
        if (newUser is null) return Conflict();
        return Ok(newUser);
    }
}