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
    
    public async Task<List<ProviderAccess>> GenerateProviderAccess(User u)
    {
        var providerAccess = new List<ProviderAccess>();
        var tokens = await tokensDao.GetOAuthTokenByUserId(u.UserId);
        foreach (var t in tokens)
        {
            string accessToken = await GenerateNewAccessToken(t);
            providerAccess.Add(new ProviderAccess(t.Provider, accessToken));
        }

        return providerAccess;
    }

    public async Task<string> GenerateNewAccessToken(OAuthToken oAuthToken)
    {
        return "dummy";
    }
}