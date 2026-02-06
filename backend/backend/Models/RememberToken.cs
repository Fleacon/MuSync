namespace backend.Models;

public class RememberToken(int rememberId, DateTime creationDate, DateTime expiryDate, string tokenHash, int userId)
{
    public int RememberId { get; init; } = rememberId;
    public DateTime CreationDate { get; init; } = creationDate;
    public DateTime ExpiryDate { get; init; } = expiryDate;
    public string TokenHash { get; init; } = tokenHash;
    public int UserId { get; init; } = userId;
}