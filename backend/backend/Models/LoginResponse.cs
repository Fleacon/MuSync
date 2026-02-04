namespace backend.Models;

public record LoginResponse(StatusCode Status, string SessionId);