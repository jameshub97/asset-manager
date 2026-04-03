namespace backend.Models;

public record CreateAssetRequest(string Name, string Description, decimal? Price);
public record UpdateAssetRequest(string? Name, string? Description, decimal? Price);