// backend/Endpoints/AssetEndpoints.cs
using System.Security.Claims;
using backend.Models;
using backend.Services;

namespace backend.Endpoints;

public static class AssetEndpoints
{
    public static void MapAssetEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/assets");
        
        // GET paged
        group.MapGet("/", (AssetService service, int page = 1, int pageSize = 8) =>
            Results.Ok(service.GetAssets(page, pageSize)));
        
        // GET single
        group.MapGet("/{id}", (string id, AssetService service) =>
        {
            var asset = service.GetAsset(id);
            return asset is null 
                ? Results.NotFound(new { message = $"Asset {id} not found" }) 
                : Results.Ok(asset);
        });
        
        // POST create
        group.MapPost("/", (CreateAssetRequest request, ClaimsPrincipal user, AssetService service) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Results.Unauthorized();
            }

            try
            {
                var asset = service.CreateAsset(request, userId);
                return Results.Created($"/api/assets/{asset.Id}", asset);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }).RequireAuthorization();
        
        // PUT update
        group.MapPut("/{id}", (string id, UpdateAssetRequest request, ClaimsPrincipal user, AssetService service) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Results.Unauthorized();
            }

            try
            {
                return service.UpdateAsset(id, request, userId) switch
                {
                    AssetMutationResult.Success => Results.Ok(service.GetAsset(id)),
                    AssetMutationResult.Forbidden => Results.Json(
                        new { message = "You can only update assets you own." },
                        statusCode: StatusCodes.Status403Forbidden),
                    _ => Results.NotFound(new { message = $"Asset {id} not found" }),
                };
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }).RequireAuthorization();
        
        // DELETE
        group.MapDelete("/{id}", (string id, ClaimsPrincipal user, AssetService service) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Results.Unauthorized();
            }

            return service.DeleteAsset(id, userId) switch
            {
                AssetMutationResult.Success => Results.NoContent(),
                AssetMutationResult.Forbidden => Results.Json(
                    new { message = "You can only delete assets you own." },
                    statusCode: StatusCodes.Status403Forbidden),
                _ => Results.NotFound(new { message = $"Asset {id} not found" }),
            };
        }).RequireAuthorization();
    }
}