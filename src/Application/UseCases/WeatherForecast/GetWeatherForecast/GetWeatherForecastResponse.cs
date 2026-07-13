namespace Application.UseCases.WeatherForecast.GetWeatherForecast;

public sealed record GetWeatherForecastResponse(IEnumerable<WeatherForecast> Forecasts);

public sealed record WeatherForecast(
    DateOnly Date,
    int TemperatureC,
    string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
