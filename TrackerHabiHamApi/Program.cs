using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TrackerHabiHamApi.Data;
using TrackerHabiHamApi.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();

// Add Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ITelegramService, TelegramService>();
builder.Services.AddScoped<IGoogleSheetsService, GoogleSheetsService>();
builder.Services.AddScoped<IWeightService, WeightService>();
builder.Services.AddScoped<ISyncService, SyncService>();
builder.Services.AddScoped<IWeightAnalysisService, WeightAnalysisService>();

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
app.UseRouting();
app.MapControllers();
app.MapOpenApi();
app.MapScalarApiReference();

app.Run();
