using Energy.WebAPI.Extensions;
using Energy.WebAPI.Hubs;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

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
app.UseCors("SignalRClient");
app.UseAuthorization();
app.MapControllers();
app.MapHub<EnergyHub>("/hubs/energy");
app.Run();
