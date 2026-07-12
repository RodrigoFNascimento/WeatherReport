using System.ComponentModel;

namespace WebAPI.Features.WeatherForecast.V1.GetWeatherForecast;

public sealed record GetWeatherForecastResponse(
    [property: Description("Weather forecasts obtained.")]
    IEnumerable<WeatherForecast> Forecasts);

public sealed record WeatherForecast(
    [property: Description("Weather forecasts date.")]
    DateOnly Date,

    [property: Description("Temperature in Celsius.")]
    int TemperatureC,

    [property: Description("Weather forecasts summary.")]
    string? Summary)
{
    [Description("Temperature in Fahrenheit.")]
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
