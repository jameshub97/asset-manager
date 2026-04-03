// backend/Services/AssetService.cs
using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public enum AssetMutationResult
{
    Success,
    NotFound,
    Forbidden
}

public class AssetService
{
    private readonly AssetDbContext _db;

    public AssetService(AssetDbContext db)
    {
        _db = db;
    }

    public PagedResponse<Asset> GetAssets(int page = 1, int pageSize = 8)
    {
        var normalizedPage = page < 1 ? 1 : page;
        var normalizedPageSize = Math.Clamp(pageSize, 1, 100);

        var query = _db.Assets
            .AsNoTracking()
            .OrderByDescending(a => a.CreatedAt);

        var totalCount = query.Count();
        var totalPages = totalCount == 0
            ? 1
            : (int)Math.Ceiling(totalCount / (double)normalizedPageSize);

        if (normalizedPage > totalPages)
        {
            normalizedPage = totalPages;
        }

        var items = query
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .ToList();

        return new PagedResponse<Asset>(
            Items: items,
            Page: normalizedPage,
            PageSize: normalizedPageSize,
            TotalCount: totalCount,
            TotalPages: totalPages,
            HasNextPage: normalizedPage < totalPages,
            HasPreviousPage: normalizedPage > 1
        );
    }

    public Asset? GetAsset(string id)
    {
        return _db.Assets.AsNoTracking().FirstOrDefault(a => a.Id == id);
    }

    public Asset CreateAsset(CreateAssetRequest request, string userId)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Name is required.");

        if (string.IsNullOrWhiteSpace(request.Description))
            throw new ArgumentException("Description is required.");

        if (!request.Price.HasValue)
            throw new ArgumentException("Price is required.");

        if (request.Price.Value < 0)
            throw new ArgumentException("Price must be 0 or greater.");

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("Authenticated user is required.");

        var asset = new Asset
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            CreatedAt = DateTime.UtcNow.ToString("o"),
            UserId = userId
        };

        _db.Assets.Add(asset);
        _db.SaveChanges();

        return asset;
    }

    public AssetMutationResult UpdateAsset(string id, UpdateAssetRequest request, string userId)
    {
        var asset = _db.Assets.FirstOrDefault(a => a.Id == id);
        if (asset is null) return AssetMutationResult.NotFound;

        if (asset.UserId != userId) return AssetMutationResult.Forbidden;

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

        return AssetMutationResult.Success;
    }

    public AssetMutationResult DeleteAsset(string id, string userId)
    {
        var asset = _db.Assets.FirstOrDefault(a => a.Id == id);
        if (asset is null) return AssetMutationResult.NotFound;

        if (asset.UserId != userId) return AssetMutationResult.Forbidden;

        _db.Assets.Remove(asset);
        _db.SaveChanges();

        return AssetMutationResult.Success;
    }
}
