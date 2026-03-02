using backend.Models;

namespace backend.Controllers;

public static class HttpContextExtensions
{
    public static User? GetCurrentUser(this HttpContext ctx) => ctx.Items["User"] as User;

    public static string? GetSessionToken(this HttpContext ctx) => ctx.Items["SessionToken"] as string;
}