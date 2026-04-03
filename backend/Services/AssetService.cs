// backend/Services/AssetService.cs
using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class AssetService
{
    private readonly AssetDbContext _db;

    public AssetService(AssetDbContext db)
    {
        _db = db;
    }

    public IEnumerable<Asset> GetAllAssets()
    {
        return _db.Assets.AsNoTracking().ToList();
    }

    public Asset? GetAsset(string id)
    {
        return _db.Assets.AsNoTracking().FirstOrDefault(a => a.Id == id);
    }

    public Asset CreateAsset(CreateAssetRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Name is required.");

        if (string.IsNullOrWhiteSpace(request.Description))
            throw new ArgumentException("Description is required.");

        if (!request.Price.HasValue)
            throw new ArgumentException("Price is required.");

        if (request.Price.Value < 0)
            throw new ArgumentException("Price must be 0 or greater.");

        var asset = new Asset
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            CreatedAt = DateTime.UtcNow.ToString("o"),
            UserId = request.UserId ?? string.Empty
        };

        _db.Assets.Add(asset);
        _db.SaveChanges();

        return asset;
    }

    public bool UpdateAsset(string id, UpdateAssetRequest request)
    {
        var asset = _db.Assets.FirstOrDefault(a => a.Id == id);
        if (asset is null) return false;

        if (request.Name is not null && string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Name cannot be empty.");

        if (request.Description is not null && string.IsNullOrWhiteSpace(request.Description))
            throw new ArgumentException("Description cannot be empty.");

        if (request.Price.HasValue && request.Price.Value < 0)
            throw new ArgumentException("Price must be 0 or greater.");

        if (request.Name is not null) asset.Name = request.Name;
        if (request.Description is not null) asset.Description = request.Description;
        if (request.Price.HasValue) asset.Price = request.Price;

        _db.SaveChanges();

        return true;
    }

    public bool DeleteAsset(string id)
    {
        var asset = _db.Assets.FirstOrDefault(a => a.Id == id);
        if (asset is null) return false;

        _db.Assets.Remove(asset);
        _db.SaveChanges();

        return true;
    }
}
