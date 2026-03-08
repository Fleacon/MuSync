using Microsoft.AspNetCore.Identity;

namespace backend.Services;

public static class PasswordService
{
    private static readonly PasswordHasher<string> hasher = new();
    
    public static string HashPassword(string pw)
    {
        return hasher.HashPassword("", pw);
    }

    public static bool VerifyPassword(string hashedPw, string providedPw)
    {
        var result = hasher.VerifyHashedPassword("", hashedPw, providedPw);
        return result == PasswordVerificationResult.Success;
    }
}