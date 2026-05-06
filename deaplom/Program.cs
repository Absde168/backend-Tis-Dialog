using deaplom.Model;
using deaplom.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// === Connection String ===
// Render даёт DATABASE_URL в формате postgres://user:pass@host:port/db
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
string connectionString;

if (!string.IsNullOrEmpty(databaseUrl))
{
    try
    {
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':');
        var host = uri.Host;
        var port = uri.Port > 0 ? uri.Port : 5432;
        var db   = uri.AbsolutePath.TrimStart('/');
        var user = userInfo[0];
        var pass = userInfo.Length > 1 ? userInfo[1] : "";
        connectionString = $"Host={host};Port={port};Database={db};Username={user};Password={pass};SSL Mode=Require;Trust Server Certificate=true";
    }
    catch
    {
        // Если URL не парсится — используем как есть (Npgsql Connection String)
        connectionString = databaseUrl;
    }
}
else
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
}

// === DbContext ===
builder.Services.AddDbContext<TisDialogContext>(options =>
    options.UseNpgsql(connectionString));

// === Services ===
builder.Services.AddScoped<IBaseService<User>, UserService>();
builder.Services.AddScoped<IBaseService<ConnectionHistory>, ConnectionHistoryService>();
builder.Services.AddScoped<IBaseService<PaymentHistory>, PaymentHistoryService>();
builder.Services.AddScoped<IBaseService<ChatMessage>, ChatMessageService>();
builder.Services.AddScoped<IBaseService<SystemNotification>, SystemNotificationService>();

// === Controllers ===
builder.Services.AddControllers();

// === Swagger ===
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "TisDialog API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] your JWT token"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// === JWT ===
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? "YourVerySecureSecretKey123!@#Loshnya";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
})
.AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer           = true,
        ValidateAudience         = true,
        ValidateLifetime         = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer              = "TisDialog",
        ValidAudience            = "MobileApp",
        IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
});

// === CORS — разрешаем всё для мобильного приложения ===
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(p =>
        p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

// === Автомиграция при старте (только если БД доступна) ===
try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<TisDialogContext>();
    db.Database.Migrate();
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogWarning("Migration failed (will retry on next start): {Message}", ex.Message);
}

// === Pipeline ===
// НЕ используем UseHttpsRedirection — Render сам терминирует HTTPS
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Health check для Render
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();
