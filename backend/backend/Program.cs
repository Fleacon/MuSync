var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Middleware
app.UseHttpsRedirection();

app.MapControllers();

app.Run();