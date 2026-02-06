using Org.BouncyCastle.Bcpg;

namespace backend.Models;

public class User(int userId, string? email, string username, string passwordHash)
{
    public int UserId { get; set; } = userId;
    public string? Email { get; set; } = email;
    public string Username { get; set; } = username;
    public string PasswordHash { get; set; } = passwordHash;
}