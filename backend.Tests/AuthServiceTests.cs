using backend.Configuration;
using backend.Data;
using backend.Models;
using backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Xunit;

namespace backend.Tests;

public class AuthServiceTests
{
    private static AssetDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AssetDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AssetDbContext(options);
    }

    private static AuthService CreateService(AssetDbContext db)
    {
        return new AuthService(db, Options.Create(new JwtOptions()));
    }

    [Fact]
    public void Register_ShouldPersistUser_WhenInputIsValid()
    {
        using var db = CreateContext();
        var service = CreateService(db);

        var result = service.Register(new RegisterRequest("alice", "alice@example.com", "Pass123"));

        Assert.True(result.Success);
        Assert.Null(result.Error);
        Assert.Equal(1, db.Users.Count());
        Assert.Equal("alice", db.Users.First().Username);
    }

    [Fact]
    public void Register_ShouldFail_WhenEmailIsInvalid()
    {
        using var db = CreateContext();
        var service = CreateService(db);

        var result = service.Register(new RegisterRequest("bob", "not-an-email", "Pass123"));

        Assert.False(result.Success);
        Assert.Equal("Email format is invalid.", result.Error);
        Assert.Empty(db.Users);
    }

    [Fact]
    public void Register_ShouldFail_WhenUsernameExists()
    {
        using var db = CreateContext();
        var service = CreateService(db);

        _ = service.Register(new RegisterRequest("charlie", "charlie@example.com", "Pass123"));
        var second = service.Register(new RegisterRequest("charlie", "charlie2@example.com", "Pass123"));

        Assert.False(second.Success);
        Assert.Equal("Username or email already exists.", second.Error);
        Assert.Equal(1, db.Users.Count());
    }
}
