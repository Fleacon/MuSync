using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace backend.Filter;

public class SessionAuthFilter : IAsyncActionFilter
{
    private readonly SessionService sessionService;
    private readonly CookieService cookieService;

    public SessionAuthFilter(SessionService sessionService)
    {
        this.sessionService = sessionService;
    }
    
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Cookies.TryGetValue("Session", out var token))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var user = await sessionService.GetUserBySessionToken(token);
        if (user is null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var refreshedSession = await sessionService.RefreshSession(token);
        cookieService.SetSession(context.HttpContext.Response, token, refreshedSession!.ExpiryDate);

        context.HttpContext.Items["user"] = user;
        context.HttpContext.Items["SessionToken"] = token;

        await next();
    }
}