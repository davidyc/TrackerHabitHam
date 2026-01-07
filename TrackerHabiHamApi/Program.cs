using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Scalar.AspNetCore;
using TrackerHabiHamApi.Data;
using TrackerHabiHamApi.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Tracker HabiHam API",
        Version = "v1"
    });
});
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();

// Add Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add MongoDB
var mongoConnectionString = builder.Configuration["Mongo:ConnectionString"] ?? "mongodb://localhost:27017";
var mongoDatabaseName = builder.Configuration["Mongo:Database"] ?? "TrackerHabiHam";

builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoConnectionString));
builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoDatabaseName);
});

builder.Services.AddScoped<ITelegramService, TelegramService>();
builder.Services.AddScoped<IGoogleSheetsService, GoogleSheetsService>();
builder.Services.AddScoped<IWeightService, WeightService>();
builder.Services.AddScoped<ISyncService, SyncService>();
builder.Services.AddScoped<IWeightAnalysisService, WeightAnalysisService>();
builder.Services.AddScoped<IJsonStoreService, JsonStoreService>();

var app = builder.Build();

// Auto migrate database on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        context.Database.Migrate();
        Console.WriteLine("Database migration completed successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while migrating the database: {ex.Message}");
    }
}

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Tracker HabiHam API v1");
});
app.UseRouting();
app.MapControllers();
app.MapOpenApi();
app.MapScalarApiReference();

app.Run();
