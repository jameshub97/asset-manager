// backend/Services/AssetService.cs
using backend.Data;
using backend.Models;

namespace backend.Services;

public class AssetService
{
    public IEnumerable<Asset> GetAllAssets() => AssetStore.GetAll();
    
    public Asset? GetAsset(string id) => AssetStore.GetById(id);
    
    public Asset CreateAsset(CreateAssetRequest request)
    {
        var asset = new Asset
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            CreatedAt = DateTime.UtcNow.ToString("o")
        };
        AssetStore.Add(asset);
        return asset;
    }
    
    public bool UpdateAsset(string id, UpdateAssetRequest request) => AssetStore.Update(id, request);
    
    public bool DeleteAsset(string id) => AssetStore.Remove(id);
}