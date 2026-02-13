using System.Text.Json;
using backend.DB.DAO;
using backend.Models;

namespace backend;

public class TokenManager
{
    private OAuthTokensDAO tokensDao;
    
    public TokenManager(OAuthTokensDAO tokensDao)
    {
        this.tokensDao = tokensDao;
    }
    
    public async Task GenerateProviderAccess(HttpResponse response, User u)
    {
        var providerAccess = new List<ProviderAccess>();
        var tokens = await tokensDao.GetOAuthTokenByUserId(u.UserId);
        foreach (var t in tokens)
        {
            string accessToken = await GenerateNewAccessToken(t);
            providerAccess.Add(new ProviderAccess(t.Provider, accessToken));
        }
        
        response.Cookies.Append("ProviderAccess", JsonSerializer.Serialize(providerAccess), new () 
        { 
            HttpOnly = true, 
            Secure = true, 
            SameSite = SameSiteMode.Lax, 
            Path = "/"
        });
    }

    private async Task<string> GenerateNewAccessToken(OAuthToken oAuthToken)
    {
        return "dummy";
    }
}