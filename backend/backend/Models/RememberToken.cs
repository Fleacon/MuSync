namespace backend.Models;

public class RememberToken(int rememberId, DateTime creationDate, DateTime expiryDate, string tokenHash, int userId)
{
    public int RememberId { get; set; } = rememberId;
    public DateTime CreationDate { get; set; } = creationDate;
    public DateTime ExpiryDate { get; set; } = expiryDate;
    public string TokenHash { get; set; } = tokenHash;
    public int UserId { get; set; } = userId;
}