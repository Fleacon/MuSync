namespace backend.Models;

public class Sessions(int sessionId, DateTime creationDate, DateTime expiryDate, int userId)
{
    public int SessionId { get; init; } = sessionId;
    public DateTime CreationDate { get; init; } = creationDate;
    public DateTime ExpiryDate { get; init; } = expiryDate;
    public int UserId { get; init; } = userId;
}