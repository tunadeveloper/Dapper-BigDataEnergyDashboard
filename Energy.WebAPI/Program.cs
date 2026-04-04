using Energy.WebAPI.Context;
using Energy.WebAPI.Extensions;
using Energy.WebAPI.Hubs;
using Energy.WebAPI.Repositories.Dashboard;
using Energy.WebAPI.Repositories.MeterReadings;
using Energy.WebAPI.Repositories.Meters;
using Energy.WebAPI.Repositories.Regions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddOpenApi();
builder.Services.AddScoped<DapperContext>();
builder.Services.AddRedisCache(builder.Configuration);
builder.Services.AddScoped<IMeterService, MeterService>();
builder.Services.AddScoped<IRegionService, RegionService>();
builder.Services.AddScoped<IMeterReadingService, MeterReadingService>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(opt =>
    {
        opt
        .WithTheme(ScalarTheme.BluePlanet)
        .WithTitle("EnergyAPI")
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHub<EnergyHub>("/hubs/energy");
app.Run();
