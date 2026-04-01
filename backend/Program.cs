var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

// ✅ ADD CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowViteDev", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",   // Vite default HTTP
                "https://localhost:5173",  // Vite HTTPS (if used)
                "http://localhost:3000",   // Alternative dev port
                "https://localhost:5002"   // Your frontend if different
              )
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowViteDev");

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// ✅ ADD YOUR ASSETS API (example)
app.MapGet("/api/assets", () =>
{
    return new[] 
    {
        new { Id = "1", Name = "Asset 1", Description = "Test", Price = 100, CreatedAt = DateTime.UtcNow },
        new { Id = "2", Name = "Asset 2", Description = "Test 2", Price = 200, CreatedAt = DateTime.UtcNow }
    };
});

app.MapPost("/api/assets", (CreateAssetRequest request) =>
{
    var newAsset = new 
    { 
        Id = Guid.NewGuid().ToString(),
        Name = request.Name,
        Description = request.Description,
        Price = request.Price,
        CreatedAt = DateTime.UtcNow
    };
    return Results.Created($"/api/assets/{newAsset.Id}", newAsset);
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

record CreateAssetRequest(string Name, string Description, decimal Price);