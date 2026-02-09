namespace backend.Models;

public class Session(int sessionId, DateTime creationDate, DateTime expiryDate, int userId, string sessionHash)
{
    public int SessionId { get; set; } = sessionId;
    public DateTime CreationDate { get; set; } = creationDate;
    public DateTime ExpiryDate { get; set; } = expiryDate;
    public int UserId { get; set; } = userId;
    public string SessionHash { get; set; } = sessionHash;
}