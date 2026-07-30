using System.ComponentModel;

namespace WebAPI.Features.WeatherForecast.V1.GetWeatherForecast;

public sealed record GetWeatherForecastResponse(
    [property: Description("Weather forecasts obtained.")]
    IEnumerable<WeatherForecast> Forecasts);

public sealed record WeatherForecast(
    [property: Description("Weather forecast date.")]
    DateOnly Date,

    [property: Description("Temperature in Celsius.")]
    double TemperatureC)
{
    [property: Description("A human-readable description of the weather.")]
    public string Summary => TemperatureC switch
    {
        < -10 => "Freezing",
        >= -10 and < 0 => "Bracing",
        >= 0 and < 10 => "Chilly",
        >= 10 and < 15 => "Cool",
        >= 15 and < 20 => "Mild",
        >= 20 and < 25 => "Warm",
        >= 25 and < 30 => "Balmy",
        >= 30 and < 35 => "Hot",
        >= 35 and < 40 => "Sweltering",
        _ => "Scorching"
    };

    [property: Description("Temperature in Fahrenheit.")]
    public double TemperatureF => 32 + (TemperatureC / 0.5556);
};
