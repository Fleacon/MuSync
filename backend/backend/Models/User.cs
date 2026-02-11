using Org.BouncyCastle.Bcpg;

namespace backend.Models;

public class User(int userId, string username, string passwordHash)
{
    public int UserId { get; set; } = userId;
    public string Username { get; set; } = username;
    public string PasswordHash { get; set; } = passwordHash;
}