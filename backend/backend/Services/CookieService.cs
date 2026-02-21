using backend.Models;

namespace backend.Services;

public class CookieService
{
    public void SetAccessToken(HttpResponse httpResponse, Provider provider, OAuthResult token)
    {
        httpResponse.Cookies.Append($"AccessToken_{provider.ToString()}", token.AccessToken, new ()
        {
            HttpOnly = true, 
            Secure = true, 
            SameSite = SameSiteMode.Lax, 
            Path = "/",
            Expires = token.Expiry
        });
    }

    public void SetSession(HttpResponse httpResponse, string token, DateTime expiryDate)
    {
        httpResponse.Cookies.Append("Session", token, new () 
        { 
            HttpOnly = true, 
            Secure = true, 
            SameSite = SameSiteMode.Lax, 
            Expires = expiryDate,
            Path = "/"
        });
    }
}