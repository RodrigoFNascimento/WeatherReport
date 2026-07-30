namespace Domain;

/// <summary>
/// Represents a weather forecast for a specific date including temperature and derived information.
/// </summary>
/// <param name="Date">The date of the forecast.</param>
/// <param name="Temperature">The temperature.</param>
public sealed record WeatherForecast(DateOnly Date, Temperature Temperature);

/// <summary>
/// Represents a temperature value in degrees Celsius and provides conversion to degrees Fahrenheit.
/// </summary>
public readonly record struct Temperature
{
    /// <summary>
    /// The temperature in degrees Celsius.
    /// </summary>
    public double DegreesCelsius { get; }

    /// <summary>
    /// The temperature in degrees Fahrenheit.
    /// </summary>
    public double DegreesFahrenheit => 32 + (DegreesCelsius / 0.5556);

    private Temperature(double degreesCelsius) => DegreesCelsius = degreesCelsius;

    /// <summary>
    /// Creates a Temperature instance from a value in degrees Celsius.
    /// </summary>
    /// <param name="value">Value in degrees Celsius.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is below absolute zero.</exception>
    public static Temperature FromCelsius(double value)
    {
        if (value < -273.15)
            throw new ArgumentOutOfRangeException(nameof(value), "Temperature below absolute zero.");

        return new Temperature(value);
    }
}