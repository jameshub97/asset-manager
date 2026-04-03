// backend/Program.cs
using backend.Endpoints;
using backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
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

// Register your business services
builder.Services.AddSingleton<AssetService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowViteDev");
app.UseHttpsRedirection();

// Map your endpoints
app.MapAssetEndpoints();

app.Run();