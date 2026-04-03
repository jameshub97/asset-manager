// backend/Services/AuthService.cs
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using backend.Configuration;
using backend.Data;
using backend.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace backend.Services;

public class AuthService
{
    private readonly AssetDbContext _db;
    private readonly JwtOptions _jwtOptions;

    public AuthService(AssetDbContext db, IOptions<JwtOptions> jwtOptions)
    {
        _db = db;
        _jwtOptions = jwtOptions.Value;
    }
    
    public (bool Success, string? Error) Register(RegisterRequest request)
    {
        var username = request.Username.Trim();
        var email = request.Email.Trim();
        var password = request.Password;

        if (string.IsNullOrWhiteSpace(username))
            return (false, "Username is required.");

        if (string.IsNullOrWhiteSpace(email))
            return (false, "Email is required.");

        if (!IsValidEmail(email))
            return (false, "Email format is invalid.");

        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            return (false, "Password must be at least 6 characters.");

        if (_db.Users.Any(u => u.Username == username || u.Email == email))
            return (false, "Username or email already exists.");

        _db.Users.Add(new User
        {
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        });

        _db.SaveChanges();
        return (true, null);
    }
    
    public AuthResponse? Login(LoginRequest request)
    {
        var username = request.Username.Trim();
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(request.Password))
            return null;

        var user = _db.Users.AsNoTracking().FirstOrDefault(u => u.Username == username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;
        
        var token = GenerateJwtToken(user);
        return new AuthResponse(token, user.Username, user.Id);
    }
    
    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_jwtOptions.ExpirationHours),
            signingCredentials: creds
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            _ = new MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }
}