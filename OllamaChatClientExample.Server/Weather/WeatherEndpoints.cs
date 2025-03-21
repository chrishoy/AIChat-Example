using OllamaChatClientExample.Server.Weather;

namespace OllamaChatClientExample.Server.Chat;

public static class WeatherEndpoints
{
    public static void MapWeatherEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/weather");

        group.MapGet("/", GetWeather).WithName("GetWeather");
    }


    private static async Task<IResult> GetWeather(IWeatherService weatherService)
    {
        var forecast = await weatherService.GetForecast();
        return forecast is not null ? Results.Ok(forecast) : Results.NotFound();
    }
}
