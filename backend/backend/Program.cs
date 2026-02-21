using System.Text.Json.Serialization;
using backend;
using backend.DB;
using backend.DB.DAO;
using backend.Services;
using DotNetEnv;
using Microsoft.AspNetCore.DataProtection;

Env.Load();

var connString = Env.GetString("DB_CONNECTION") ?? throw new InvalidOperationException("DB_CONNECTION not found");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddScoped<DbManager>(_ => new (connString));
builder.Services.AddScoped<UsersDAO>();
builder.Services.AddScoped<SessionsDAO>();
builder.Services.AddScoped<RememberTokensDAO>();
builder.Services.AddScoped<OAuthTokensDAO>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ProviderApiService>();
builder.Services.AddScoped<CookieService>();

builder.Services.AddDataProtection().PersistKeysToFileSystem(new("/sec/keys/"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();  
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();