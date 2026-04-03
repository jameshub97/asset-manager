// backend/Data/AssetStore.cs
using backend.Models;

namespace backend.Data;

public static class AssetStore
{
    // ✅ Use traditional initialization for compatibility
    private static readonly List<Asset> _assets = new()
    {
        new Asset 
        { 
            Id = "1", 
            Name = "Asset 1", 
            Description = "Test", 
            Price = 100, 
            CreatedAt = DateTime.UtcNow.ToString("o") 
        },
        new Asset 
        { 
            Id = "2", 
            Name = "Asset 2", 
            Description = "Test 2", 
            Price = 200, 
            CreatedAt = DateTime.UtcNow.ToString("o") 
        }
    };

    public static List<Asset> GetAll() => _assets;
    
    public static Asset? GetById(string id) => _assets.FirstOrDefault(a => a.Id == id);
    
    public static void Add(Asset asset) => _assets.Add(asset);
    
    public static bool Remove(string id)
    {
        var asset = GetById(id);
        if (asset is null) return false;
        return _assets.Remove(asset);
    }
    
    public static bool Update(string id, UpdateAssetRequest request)
    {
        var asset = GetById(id);
        if (asset is null) return false;
        
        // Update only the fields that are provided
        if (request.Name is not null)
            asset.Name = request.Name;
            
        if (request.Description is not null)
            asset.Description = request.Description;
            
        if (request.Price.HasValue)
            asset.Price = request.Price;
        
        return true;
    }
}