namespace backend.Models;

public record CreateAssetRequest(string Name, string Description, decimal? Price);
public record UpdateAssetRequest(string? Name, string? Description, decimal? Price);
public record PagedResponse<T>(
	IReadOnlyList<T> Items,
	int Page,
	int PageSize,
	int TotalCount,
	int TotalPages,
	bool HasNextPage,
	bool HasPreviousPage);