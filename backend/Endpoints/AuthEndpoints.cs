// backend/Endpoints/AuthEndpoints.cs
using backend.Models;
using backend.Services;

namespace backend.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");
        
        group.MapPost("/register", (RegisterRequest request, AuthService service) =>
        {
            var result = service.Register(request);
            return result.Success
                ? Results.Ok(new { message = "User registered successfully" })
                : Results.BadRequest(new { message = result.Error ?? "Registration failed" });
        });
        
        group.MapPost("/login", (LoginRequest request, AuthService service) =>
        {
            var response = service.Login(request);
            return response is not null 
                ? Results.Ok(response)
                : Results.Unauthorized();
        });
    }
}