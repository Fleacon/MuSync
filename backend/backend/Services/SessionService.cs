using System.Security.Cryptography;
using System.Text;
using backend.DB.DAO;
using backend.Models;

namespace backend.Services;

public class SessionService
{
    private readonly SessionsDAO sessionsDao;
    private readonly UsersDAO usersDao;

    private int expiryTime = 30; // In Minutes 

    public SessionService(SessionsDAO sessionsDao, UsersDAO usersDao)
    {
        this.sessionsDao = sessionsDao;
        this.usersDao = usersDao;
    }

    public async Task<Session> GenerateSession(int uId, string token)
    {
        var creationDate = DateTime.Now;
        var expiryDate = creationDate.AddMinutes(expiryTime);
        var sessionHash = HashSessionToken(token);

        return await sessionsDao.CreateSession(new(0, creationDate, expiryDate, uId, sessionHash));
    }

    public string GenerateSessionToken()
    {
        byte[] bytes = new byte[32]; // 256 bits
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    public async Task<bool> DeleteSession(string token)
    {
        var hashedToken = HashSessionToken(token);
        var session = await sessionsDao.GetSessionByHash(hashedToken);
        if (session is null)
            return false;
        await sessionsDao.RemoveSessionById(session.SessionId);
        return true;
    }
    
    public async Task<User?> GetUserBySessionToken(string rawToken)
    {
        var hash = HashSessionToken(rawToken);
        return await usersDao.GetUserByHashedSessionToken(hash);
    }

    public async Task<Session?> RefreshSession(string token)
    {
        var hashedToken = HashSessionToken(token);
        var session = await sessionsDao.GetSessionByHash(hashedToken);
        if (session is null)
            return null;
        var newExpiryDate = DateTime.Now.AddMinutes(expiryTime);
        return await sessionsDao.UpdateExpiryDateById(session.SessionId, newExpiryDate);
    }

    public static string HashSessionToken(string token)
    {
        using var sha256 = SHA256.Create();
        byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hash);
    }
}