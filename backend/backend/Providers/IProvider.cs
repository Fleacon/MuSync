using backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.Providers;

public interface IProvider
{
    public Provider Provider { get; }
    public ActionResult AuthRequest(HttpContext httpContext);
    public Task<OAuthResult> HandleCallbackAsync(HttpContext httpContext);
}