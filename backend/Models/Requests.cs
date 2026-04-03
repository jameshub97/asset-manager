namespace backend.Models;

public record CreateAssetRequest(string Name, string Description, decimal? Price, string? UserId);
public record UpdateAssetRequest(string? Name, string? Description, decimal? Price);