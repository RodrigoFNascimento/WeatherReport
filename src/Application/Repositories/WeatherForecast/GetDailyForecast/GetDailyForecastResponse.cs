namespace Application.Repositories.WeatherForecast.GetDailyForecast;

/// <summary>
/// Contains the location that was requested and the associated daily forecast data.
/// </summary>
/// <param name="Latitude">Latitude of the forecast location in decimal degrees.</param>
/// <param name="Longitude">Longitude of the forecast location in decimal degrees.</param>
/// <param name="Daily">Daily forecast data for the location. Each element in the arrays of <see cref="DailyData"/> corresponds to the same day.</param>
public sealed record GetDailyForecastResponse(
    double Latitude,
    double Longitude,
    DailyData Daily);

/// <summary>
/// Container for arrays of daily values returned by the weather provider.
/// The arrays are parallel: values at the same index correspond to the same day.
/// </summary>
/// <param name="Time">Array of date strings for each day (for example, ISO 8601 date or datetime strings).</param>
/// <param name="Temperature_2m_Max">Array of daily maximum temperatures measured at 2 meters above ground. Units are provider-dependent; consumers should interpret according to the data source.</param>
public sealed record DailyData(
    string[] Time,
    double[] Temperature_2m_Max
);