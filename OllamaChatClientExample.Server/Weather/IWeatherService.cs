namespace OllamaChatClientExample.Server.Weather;

public interface IWeatherService
{
    Task<WeatherForecast[]> GetForecast();
}