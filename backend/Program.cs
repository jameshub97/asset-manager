// backend/Program.cs
using backend.Data;
using backend.Endpoints;
using backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
    options.AddPolicy("AllowViteDev", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

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
            ValidIssuer = "asset-manager",
            ValidAudience = "asset-manager-users",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-super-secret-jwt-key-here-make-it-long-and-secure"))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowViteDev");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Map your endpoints
app.MapAssetEndpoints();
app.MapAuthEndpoints();

app.Run();