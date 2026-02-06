namespace backend.Models;

public class OAuthTokens(int oAuthId, int providerUserId, Provider provider, string refreshToken, string userId)
{
    public int OAuthId { get; init; } = oAuthId;
    public int ProviderUserId { get; init; } = providerUserId;
    public Provider Provider { get; init; } = provider;
    public string RefreshToken { get; init; } = refreshToken;
    public string UserId { get; init; } = userId;
}