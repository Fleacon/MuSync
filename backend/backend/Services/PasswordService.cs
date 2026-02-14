using Microsoft.AspNetCore.Identity;

namespace backend.Services;

public static class PasswordService
{
    private static readonly PasswordHasher<string> hasher = new();
    
    public static string HashPassword(string pw)
    {
        return hasher.HashPassword(null, pw);
    }

    public static bool VerifyPassword(string hashedPw, string providedPw)
    {
        var result = hasher.VerifyHashedPassword(null, hashedPw, providedPw);
        return result == PasswordVerificationResult.Success;
    }
}