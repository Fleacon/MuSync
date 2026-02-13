using System.Security.Cryptography;
using System.Text;
using backend.DB.DAO;
using backend.Models;

namespace backend;

public class SessionManager
{
    private readonly SessionsDAO sDao;

    public SessionManager(SessionsDAO sDao)
    {
        this.sDao = sDao;
    }
    
    public async Task<Session> GenerateSession(HttpResponse response, int uId)
    {
        var creationDate = DateTime.Now;
        var expiryDate = creationDate.AddHours(24);
        
        string token = GenerateSessionToken();
        var sessionHash = HashSessionToken(token);
        
        response.Cookies.Append("Session", token, new () 
        { 
            HttpOnly = true, 
            Secure = true, 
            SameSite = SameSiteMode.Lax, 
            Expires = expiryDate,
            Path = "/"
        });

        return await sDao.CreateSession(new(0, creationDate, expiryDate, uId, sessionHash));
    }
    
    public string GenerateSessionToken()
    {
        byte[] bytes = new byte[32]; // 256 bits
        RandomNumberGenerator.Fill(bytes);

        return Convert.ToBase64String(bytes);
    }

    public bool VerifySessionToken(string hashedToken, string providedToken)
    {
        string hash = HashSessionToken(providedToken);
        return hashedToken == hash;
    }

    public async Task<bool> DeleteSession(string token)
    {
        var hashedToken = HashSessionToken(token);
        var session = await sDao.GetSessionByHash(hashedToken);
        if (session is null)
            return false;
        await sDao.RemoveSessionById(session.SessionId);
        return true;
    }
    
    private string HashSessionToken(string token)
    {
        using var sha256 = SHA256.Create();
        byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hash);
    }
}