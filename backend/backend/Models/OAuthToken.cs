namespace backend.Models;

public class OAuthToken(int oAuthId, Provider provider, string refreshToken, int userId)
{
    public int OAuthId { get; set; } = oAuthId;
    public Provider Provider { get; set; } = provider;
    public string RefreshToken { get; set; } = refreshToken;
    public int UserId { get; set; } = userId;
}