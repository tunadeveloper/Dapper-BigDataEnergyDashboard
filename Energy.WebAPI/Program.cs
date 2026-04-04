using Energy.WebAPI.Context;
using Energy.WebAPI.Extensions;
using Energy.WebAPI.Repositories.MeterReadings;
using Energy.WebAPI.Repositories.Meters;
using Energy.WebAPI.Repositories.Regions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddScoped<DapperContext>();
builder.Services.AddRedisCache(builder.Configuration);
builder.Services.AddScoped<IMeterService, MeterService>();
builder.Services.AddScoped<IRegionService, RegionService>();
builder.Services.AddScoped<IMeterReadingService, MeterReadingService>();

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
app.Run();
