namespace backend.Models;

public class Session(int sessionId, DateTime creationDate, DateTime expiryDate, int userId)
{
    public int SessionId { get; set; } = sessionId;
    public DateTime CreationDate { get; set; } = creationDate;
    public DateTime ExpiryDate { get; set; } = expiryDate;
    public int UserId { get; set; } = userId;
}