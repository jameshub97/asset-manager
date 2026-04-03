// backend/Endpoints/AssetEndpoints.cs
using backend.Models;
using backend.Services;

namespace backend.Endpoints;

public static class AssetEndpoints
{
    public static void MapAssetEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/assets").RequireAuthorization();
        
        // GET all
        group.MapGet("/", (AssetService service) => 
            Results.Ok(service.GetAllAssets()));
        
        // GET single
        group.MapGet("/{id}", (string id, AssetService service) =>
        {
            var asset = service.GetAsset(id);
            return asset is null 
                ? Results.NotFound(new { message = $"Asset {id} not found" }) 
                : Results.Ok(asset);
        });
        
        // POST create
        group.MapPost("/", (CreateAssetRequest request, AssetService service) =>
        {
            try
            {
                var asset = service.CreateAsset(request);
                return Results.Created($"/api/assets/{asset.Id}", asset);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        });
        
        // PUT update
        group.MapPut("/{id}", (string id, UpdateAssetRequest request, AssetService service) =>
        {
            try
            {
                return service.UpdateAsset(id, request)
                    ? Results.Ok(service.GetAsset(id))
                    : Results.NotFound();
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        });
        
        // DELETE
        group.MapDelete("/{id}", (string id, AssetService service) =>
        {
            return service.DeleteAsset(id) 
                ? Results.NoContent()
                : Results.NotFound();
        });
    }
}