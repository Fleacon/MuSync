using backend.Models;
using backend.Providers;
using Microsoft.AspNetCore.Mvc;

namespace backend.Services;

public class AuthService
{
    private readonly IDictionary<Provider, IProvider> providers;

    public AuthService(IEnumerable<IProvider> providers)
    {
        this.providers = providers.ToDictionary(p => p.Provider);
    }

    public ActionResult RequestAuth(Provider prov, HttpContext ctx)
    {
        if (!providers.TryGetValue(prov, out var handler))
            return new BadRequestResult();

        return handler.AuthRequest(ctx);
    }
    
    public async Task<OAuthResult?> HandleCallback(Provider provider, HttpContext httpContext)
    {
        if (!providers.TryGetValue(provider, out var handler))
            return null;

        return await handler.HandleCallbackAsync(httpContext);
    }
}