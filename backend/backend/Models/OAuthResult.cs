namespace backend.Models;

public record OAuthResult(string RefreshToken, string AccessToken, DateTime Expiry);