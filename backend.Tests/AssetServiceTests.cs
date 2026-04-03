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
    public void CreateAsset_ShouldPersistAuthenticatedOwner()
    {
        using var db = CreateContext();
        var service = new AssetService(db);

        var asset = service.CreateAsset(new CreateAssetRequest("Laptop", "Portable workstation", 1499.99m), "user-123");

        Assert.Equal("user-123", asset.UserId);
        Assert.Single(db.Assets);
        Assert.Equal("user-123", db.Assets.Single().UserId);
    }

    [Fact]
    public void CreateAsset_ShouldThrow_WhenPriceIsNegative()
    {
        using var db = CreateContext();
        var service = new AssetService(db);

        var ex = Assert.Throws<ArgumentException>(() =>
            service.CreateAsset(new CreateAssetRequest("Laptop", "Dev machine", -1m), "user-123"));

        Assert.Equal("Price must be 0 or greater.", ex.Message);
    }

    [Fact]
    public void GetAssets_ShouldReturnPagedResponse()
    {
        using var db = CreateContext();
        SeedAssets(db, 10, "owner-1");
        var service = new AssetService(db);

        var response = service.GetAssets(page: 2, pageSize: 4);

        Assert.Equal(2, response.Page);
        Assert.Equal(4, response.PageSize);
        Assert.Equal(10, response.TotalCount);
        Assert.Equal(3, response.TotalPages);
        Assert.Equal(4, response.Items.Count);
        Assert.True(response.HasNextPage);
        Assert.True(response.HasPreviousPage);
    }

    [Fact]
    public void UpdateAsset_ShouldReturnForbidden_WhenUserDoesNotOwnAsset()
    {
        using var db = CreateContext();
        var asset = SeedAsset(db, "owner-1", "Camera", "Field kit", 899m);
        var service = new AssetService(db);

        var result = service.UpdateAsset(asset.Id, new UpdateAssetRequest("Updated Camera", null, null), "owner-2");

        Assert.Equal(AssetMutationResult.Forbidden, result);
        Assert.Equal("Camera", db.Assets.Single().Name);
    }

    [Fact]
    public void UpdateAsset_ShouldPersistChanges_WhenUserOwnsAsset()
    {
        using var db = CreateContext();
        var asset = SeedAsset(db, "owner-1", "Mouse", "Wireless", 49m);
        var service = new AssetService(db);

        var result = service.UpdateAsset(asset.Id, new UpdateAssetRequest("Mouse Pro", "Wireless gaming mouse", 79m), "owner-1");

        var updated = db.Assets.Single();
        Assert.Equal(AssetMutationResult.Success, result);
        Assert.Equal("Mouse Pro", updated.Name);
        Assert.Equal("Wireless gaming mouse", updated.Description);
        Assert.Equal(79m, updated.Price);
    }

    [Fact]
    public void UpdateAsset_ShouldThrow_WhenPriceIsNegative()
    {
        using var db = CreateContext();
        var service = new AssetService(db);
        var created = service.CreateAsset(new CreateAssetRequest("Phone", "Test", 100m), "user-123");

        var ex = Assert.Throws<ArgumentException>(() =>
            service.UpdateAsset(created.Id, new UpdateAssetRequest(null, null, -10m), "user-123"));

        Assert.Equal("Price must be 0 or greater.", ex.Message);
    }

    [Fact]
    public void DeleteAsset_ShouldRemoveAsset_WhenUserOwnsAsset()
    {
        using var db = CreateContext();
        var asset = SeedAsset(db, "owner-1", "Keyboard", "Mechanical", 129m);
        var service = new AssetService(db);

        var result = service.DeleteAsset(asset.Id, "owner-1");

        Assert.Equal(AssetMutationResult.Success, result);
        Assert.Empty(db.Assets);
    }

    [Fact]
    public void DeleteAsset_ShouldReturnForbidden_WhenUserDoesNotOwnAsset()
    {
        using var db = CreateContext();
        var asset = SeedAsset(db, "owner-1", "Monitor", "4K display", 399m);
        var service = new AssetService(db);

        var result = service.DeleteAsset(asset.Id, "owner-2");

        Assert.Equal(AssetMutationResult.Forbidden, result);
        Assert.Single(db.Assets);
    }

    private static void SeedAssets(AssetDbContext db, int count, string userId)
    {
        for (var index = 0; index < count; index++)
        {
            db.Assets.Add(new Asset
            {
                Id = $"asset-{index}",
                Name = $"Asset {index}",
                Description = "Seeded asset",
                Price = index + 1,
                CreatedAt = DateTime.UtcNow.AddMinutes(index).ToString("o"),
                UserId = userId,
            });
        }

        db.SaveChanges();
    }

    private static Asset SeedAsset(AssetDbContext db, string userId, string name, string description, decimal price)
    {
        var asset = new Asset
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Description = description,
            Price = price,
            CreatedAt = DateTime.UtcNow.ToString("o"),
            UserId = userId,
        };

        db.Assets.Add(asset);
        db.SaveChanges();
        return asset;
    }
}
