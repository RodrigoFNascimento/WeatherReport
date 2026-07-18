namespace Domain;

/// <summary>
/// Represents a weather forecast for a specific date including temperature and derived information.
/// </summary>
/// <param name="Date">The date of the forecast.</param>
/// <param name="TemperatureC">The temperature in degrees Celsius.</param>
public sealed record WeatherForecast(
    DateOnly Date,
    double TemperatureC)
{
    /// <summary>
    /// The temperature in degrees Fahrenheit calculated from <see cref="TemperatureC"/>.
    /// </summary>
    public double TemperatureF => 32 + (TemperatureC / 0.5556);

    /// <summary>
    /// A human-readable summary describing the weather based on <see cref="TemperatureC"/>.
    /// </summary>
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
}
