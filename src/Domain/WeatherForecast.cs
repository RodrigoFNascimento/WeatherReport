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
}
