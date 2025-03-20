using OllamaChatClientExample.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IWeatherService, WeatherService>();

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/weather", async (IWeatherService weatherService) =>
{
    var forecast = await weatherService.GetForecast();
    return forecast is null ? Results.NotFound() : Results.Ok(forecast);
})
.WithName("GetWeather");

app.MapFallbackToFile("/index.html");

app.Run();
