var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowViteDev", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowViteDev");
app.UseHttpsRedirection();

var assets = new List<Asset>
{
    new Asset { Id = "1", Name = "Asset 1", Description = "Test", Price = 100, CreatedAt = DateTime.UtcNow.ToString("o") },
    new Asset { Id = "2", Name = "Asset 2", Description = "Test 2", Price = 200, CreatedAt = DateTime.UtcNow.ToString("o") }
};

// GET all - uses shared list
app.MapGet("/api/assets", () => Results.Ok(assets));

// GET single - uses shared list
app.MapGet("/api/assets/{id}", (string id) =>
{
    var asset = assets.FirstOrDefault(a => a.Id == id);
    return asset is null 
        ? Results.NotFound(new { message = $"Asset {id} not found" }) 
        : Results.Ok(asset);
});

// POST - adds to shared list ✅
app.MapPost("/api/assets", (CreateAssetRequest request) =>
{
    var newAsset = new Asset
    { 
        Id = Guid.NewGuid().ToString(),
        Name = request.Name,
        Description = request.Description,
        Price = request.Price,
        CreatedAt = DateTime.UtcNow.ToString("o")
    };
    
    assets.Add(newAsset);  // ✅ Now in shared list!
    
    return Results.Created($"/api/assets/{newAsset.Id}", newAsset);
});

// PUT - update in shared list
app.MapPut("/api/assets/{id}", (string id, UpdateAssetRequest req) =>
{
    var asset = assets.FirstOrDefault(a => a.Id == id);
    if (asset is null) return Results.NotFound();
    
    asset.Name = req.Name ?? asset.Name;
    asset.Description = req.Description ?? asset.Description;
    asset.Price = req.Price ?? asset.Price;
    
    return Results.Ok(asset);
});

// DELETE - remove from shared list
app.MapDelete("/api/assets/{id}", (string id) =>
{
    var asset = assets.FirstOrDefault(a => a.Id == id);
    if (asset is null) return Results.NotFound();
    
    assets.Remove(asset);
    return Results.NoContent();
});

app.Run();

class Asset
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public string? CreatedAt { get; set; }
}

record CreateAssetRequest(string Name, string Description, decimal? Price);
record UpdateAssetRequest(string? Name, string? Description, decimal? Price);

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}