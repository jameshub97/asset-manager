// backend/Services/AuthService.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Data;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Services;

public class AuthService
{
    private readonly string _jwtSecret = "your-super-secret-jwt-key-here-make-it-long-and-secure"; // Move to config later
    private readonly AssetDbContext _db;

    public AuthService(AssetDbContext db)
    {
        _db = db;
    }
    
    public bool Register(RegisterRequest request)
    {
        var username = request.Username.Trim();
        var email = request.Email.Trim();

        if (_db.Users.Any(u => u.Username == username || u.Email == email))
            return false;

        _db.Users.Add(new User
        {
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        });

        _db.SaveChanges();
        return true;
    }
    
    public AuthResponse? Login(LoginRequest request)
    {
        var user = _db.Users.AsNoTracking().FirstOrDefault(u => u.Username == request.Username);
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
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: "asset-manager",
            audience: "asset-manager-users",
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}