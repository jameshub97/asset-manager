// backend/Program.cs
using backend.Configuration;
using backend.Data;
using backend.Endpoints;
using backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddOpenApi();

// Configure PostgreSQL connection
var postgresConnection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Port=5432;Database=AssetManagerDb;Username=postgres;Password=postgres";

builder.Services.AddDbContext<AssetDbContext>(options =>
    options.UseNpgsql(postgresConnection));

// Register your business services
builder.Services.AddScoped<AssetService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddCors(options =>
{
    var localDevOrigins = new[]
    {
        "http://localhost:5173",
        "http://localhost:5174",
        "http://localhost:5175",
        "http://127.0.0.1:5173",
        "http://127.0.0.1:5174",
        "http://127.0.0.1:5175"
    };

    var configuredOrigins = builder.Configuration
        .GetSection("Cors:AllowedOrigins")
        .Get<string[]>() ?? Array.Empty<string>();

    var allowedOrigins = localDevOrigins
        .Concat(configuredOrigins)
        .Where(origin => !string.IsNullOrWhiteSpace(origin))
        .Select(origin => origin.TrimEnd('/'))
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToArray();

    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();

if (!builder.Environment.IsDevelopment() && jwtOptions.Secret == "development-only-super-secret-jwt-key-change-me")
{
    throw new InvalidOperationException("Jwt:Secret must be configured for non-development environments.");
}

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Apply pending EF Core migrations at startup with retry for Docker DB readiness.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AssetDbContext>();
    if (dbContext.Database.IsRelational())
    {
        const int maxAttempts = 10;
        var attempt = 0;

        while (true)
        {
            try
            {
                dbContext.Database.Migrate();
                break;
            }
            catch (NpgsqlException) when (attempt < maxAttempts)
            {
                attempt++;
                Thread.Sleep(2000);
            }
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Map your endpoints
app.MapAssetEndpoints();
app.MapAuthEndpoints();

app.Run();

public partial class Program;