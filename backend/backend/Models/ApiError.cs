namespace backend.Models;

public record ApiError(int code, string message, string link = null!)
{
    private static readonly string SwaggerLink = Environment.GetEnvironmentVariable("SWAGGER_LINK") 
                                                 ?? "https://127.0.0.1:7129/swagger/index.html";
    
    public ApiError(int code, string message) : this(code, message, SwaggerLink) { }
}