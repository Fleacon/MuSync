namespace backend.Models;

public record ApiError(int code, string message, string link = "https://127.0.0.1:7129/swagger/index.html");