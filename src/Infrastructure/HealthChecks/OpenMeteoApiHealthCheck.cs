using Application.Repositories.WeatherForecast;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Infrastructure.HealthChecks;

internal sealed class OpenMeteoApiHealthCheck : IHealthCheck
{
    private readonly IWeatherForecastRepository _openMeteoRepository;

    public OpenMeteoApiHealthCheck(IWeatherForecastRepository openMeteoRepository) =>
        _openMeteoRepository = openMeteoRepository;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var result = await _openMeteoRepository.CheckHealthAsync(cancellationToken);

        return result.IsSuccess
            ? HealthCheckResult.Healthy("Open Meteo API is healthy.")
            : HealthCheckResult.Unhealthy("Open Meteo API is unhealthy.");
    }
}
