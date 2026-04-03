// backend/Services/AuthService.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using backend.Models;

namespace backend.Services;

public class AuthService
{
    private readonly string _jwtSecret = "your-super-secret-jwt-key-here-make-it-long-and-secure"; // Move to config later
    private readonly List<User> _users = new(); // Move to database later
    
    public bool Register(RegisterRequest request)
    {
        if (_users.Any(u => u.Username == request.Username))
            return false;
        
        _users.Add(new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        });
        return true;
    }
    
    public AuthResponse? Login(LoginRequest request)
    {
        var user = _users.FirstOrDefault(u => u.Username == request.Username);
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