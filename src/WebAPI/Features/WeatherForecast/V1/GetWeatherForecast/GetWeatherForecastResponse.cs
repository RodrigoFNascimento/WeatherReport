using System.ComponentModel;

namespace WebAPI.Features.WeatherForecast.V1.GetWeatherForecast;

public sealed record GetWeatherForecastResponse(
    [property: Description("Weather forecasts obtained.")]
    IEnumerable<WeatherForecast> Forecasts);

public sealed record WeatherForecast(
    [property: Description("Weather forecast date.")]
    DateOnly Date,

    [property: Description("Temperature in Celsius.")]
    int TemperatureC,

    [property: Description("Temperature in Fahrenheit.")]
    int TemperatureF,

    [property: Description("Weather forecast summary.")]
    string? Summary);
