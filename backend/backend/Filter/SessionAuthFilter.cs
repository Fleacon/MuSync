using backend.DB;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace backend.Filter;

public class SessionAuthFilter : IAsyncActionFilter
{
    private readonly SessionService sessionService;
    private readonly CookieService cookieService;
    private readonly RememberTokenService rememberTokenService;

    public SessionAuthFilter(SessionService sessionService, CookieService cookieService, RememberTokenService rememberTokenService)
    {
        this.sessionService = sessionService;
        this.cookieService = cookieService;
        this.rememberTokenService = rememberTokenService;
    }
    
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var hasSkip = context.ActionDescriptor.EndpointMetadata
            .OfType<SkipSessionAuthAttribute>()
            .Any();

        if (hasSkip)
        {
            await next();
            return;
        }

        try
        {
            if (!context.HttpContext.Request.Cookies.TryGetValue("Session", out var token))
            {
                if (!await TryResumeFromRememberToken(context.HttpContext))
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }
            }
            else
            {
                var user = await sessionService.GetUserBySessionToken(token);
                if (user is null)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                var refreshedSession = await sessionService.RefreshSession(token);
                cookieService.SetSession(context.HttpContext.Response, token, refreshedSession!.ExpiryDate);

                context.HttpContext.Items["User"] = user;
                context.HttpContext.Items["SessionToken"] = token;
            }
        }
        catch (DatabaseUnavailableException)
        {
            context.Result = new ObjectResult("Service temporarily unavailable. Please try again later.")
            {
                StatusCode = StatusCodes.Status503ServiceUnavailable
            };
            return;
        }

        await next();
    }
    
    private async Task<bool> TryResumeFromRememberToken(HttpContext httpContext)
    {
        if (!httpContext.Request.Cookies.TryGetValue("Remember", out var rawToken))
        {
            return false;
        }

        var user = await rememberTokenService.GetUserByTokenHash(rawToken);
        if (user is null)
        {
            return false;
        }

        await rememberTokenService.DeleteSession(rawToken);
        var newRawToken = rememberTokenService.GenerateToken();
        var newToken = await rememberTokenService.GenerateRemember(user.UserId, newRawToken);
        cookieService.SetRemember(httpContext.Response, newRawToken, newToken.ExpiryDate);

        var sessionToken = sessionService.GenerateSessionToken();
        var session = await sessionService.GenerateSession(user.UserId, sessionToken);
        cookieService.SetSession(httpContext.Response, sessionToken, session.ExpiryDate);

        httpContext.Items["User"] = user;
        httpContext.Items["SessionToken"] = sessionToken;

        return true;
    }
}

public class SkipSessionAuthAttribute : Attribute { }