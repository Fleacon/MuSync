using System.Security.Cryptography;
using System.Text;
using backend.DB.DAO;
using backend.Models;

namespace backend.Services;

public class RememberTokenService
{
    private readonly RememberTokensDAO rememberDao;
    private readonly UsersDAO usersDao;

    private int expiryTime = 30; // In Days

    public RememberTokenService(RememberTokensDAO rememberDao, UsersDAO usersDao)
    {
        this.rememberDao = rememberDao;
        this.usersDao = usersDao;
    }
    
    public async Task<RememberToken> GenerateRemember(int uId, string token)
    {
        var creationDate = DateTime.Now;
        var expiryDate = creationDate.AddDays(expiryTime);
        var sessionHash = HashSessionToken(token);

        return await rememberDao.CreateRememberToken(new(0, creationDate, expiryDate, sessionHash, uId));
    }
    
    public string GenerateToken()
    {
        byte[] bytes = new byte[32]; // 256 bits
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }
    
    public async Task<bool> DeleteSession(string token)
    {
        var hashedToken = HashSessionToken(token);
        var session = await rememberDao.GetRememberTokenByHash(hashedToken);
        if (session is null)
            return false;
        await rememberDao.DeleteRememberTokenByHash(hashedToken);
        return true;
    }

    public async Task<User?> GetUserByTokenHash(string rawToken)
    {
        var hash = HashSessionToken(rawToken);
        return await usersDao.GetUserByRememberHash(hash);
    }
    
    public static string HashSessionToken(string token)
    {
        using var sha256 = SHA256.Create();
        byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hash);
    }
}