using backend.Data;
using backend.Models;
using backend.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace backend.Tests;

public class AssetServiceTests
{
    private static AssetDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AssetDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AssetDbContext(options);
    }

    [Fact]
    public void CreateAsset_ShouldPersistWithUserId_WhenRequestIsValid()
    {
        using var db = CreateContext();
        var service = new AssetService(db);

        var asset = service.CreateAsset(new CreateAssetRequest("Laptop", "Dev machine", 1200m, "user-123"));

        Assert.Equal("user-123", asset.UserId);
        Assert.Equal(1, db.Assets.Count());
        Assert.Equal("user-123", db.Assets.First().UserId);
    }

    [Fact]
    public void CreateAsset_ShouldThrow_WhenPriceIsNegative()
    {
        using var db = CreateContext();
        var service = new AssetService(db);

        var ex = Assert.Throws<ArgumentException>(() =>
            service.CreateAsset(new CreateAssetRequest("Laptop", "Dev machine", -1m, "user-123")));

        Assert.Equal("Price must be 0 or greater.", ex.Message);
    }

    [Fact]
    public void UpdateAsset_ShouldThrow_WhenPriceIsNegative()
    {
        using var db = CreateContext();
        var service = new AssetService(db);

        var created = service.CreateAsset(new CreateAssetRequest("Phone", "Test", 100m, "user-123"));

        var ex = Assert.Throws<ArgumentException>(() =>
            service.UpdateAsset(created.Id, new UpdateAssetRequest(null, null, -10m)));

        Assert.Equal("Price must be 0 or greater.", ex.Message);
    }
}
