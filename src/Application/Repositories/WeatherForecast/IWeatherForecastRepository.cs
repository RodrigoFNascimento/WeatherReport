using FluentResults;

namespace Application.Repositories.WeatherForecast;

/// <summary>
/// Defines operations for querying the external weather forecast provider.
/// Implementations should encapsulate HTTP calls, error handling and any mapping
/// from provider-specific models into application contracts.
/// </summary>
public interface IWeatherForecastRepository
{
    /// <summary>
    /// Checks the health of the weather forecast API by making a lightweight request
    /// and validating that the provider is reachable and responding as expected.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the operation to complete.</param>
    /// <returns>
    /// A <see cref="Result"/> indicating the outcome of the health check.
    /// - On success: a successful <see cref="Result"/> (no value).
    /// - On failure: a failed <see cref="Result"/> containing one or more error reasons describing the problem.
    /// </returns>
    Task<Result> CheckHealthAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets the daily weather forecast for the specified geographic coordinates.
    /// </summary>
    /// <param name="latitude">Latitude in decimal degrees (WGS84).</param>
    /// <param name="longitude">Longitude in decimal degrees (WGS84).</param>
    /// <param name="forecastDays">The number of days to include in the forecast. Must be greater than zero and within provider limits.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the operation to complete.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing a <see cref="GetDailyForecastResponse"/> on success.
    /// On failure the <see cref="Result{T}"/> will be in a failed state and include error details (e.g., network errors, invalid parameters, provider errors).
    /// </returns>
    Task<Result<IReadOnlyCollection<Domain.WeatherForecast>>> GetDailyForecastAsync(
        double latitude,
        double longitude,
        int forecastDays,
        CancellationToken cancellationToken);
}