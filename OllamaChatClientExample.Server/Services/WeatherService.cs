using OllamaChatClientExample.Server.Models;

namespace OllamaChatClientExample.Server.Services;

public sealed class WeatherService: IWeatherService
{
    private readonly string[] summaries =
        [ "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" ];

    public Task<WeatherForecast[]> GetForecast()
    {
        // Simulate async request
        return Task.FromResult(Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray()
            );
    }
}

public interface IWeatherService
{
    Task<WeatherForecast[]> GetForecast();
}