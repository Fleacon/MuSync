using Org.BouncyCastle.Bcpg;

namespace backend.Models;

public class User(int userId, string? email, string username, string passwordHash)
{
    public int UserId { get; init; } = userId;
    public string? Email { get; init; } = email;
    public string Username { get; init; } = username;
    public string PasswordHash { get; init; } = passwordHash;
}