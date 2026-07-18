using Application.Repositories.WeatherForecast;
using Application.Repositories.WeatherForecast.GetDailyForecast;
using FluentResults;
using System.Net.Http.Json;

namespace Infrastructure.Repositories;

internal sealed class OpenMeteoApiRepository : IWeatherForecastRepository
{
    private readonly HttpClient _httpClient;

    public OpenMeteoApiRepository(HttpClient httpClient) =>
        _httpClient = httpClient;

    public async Task<Result> CheckHealthAsync(CancellationToken cancellationToken)
    {
        var requestUri = "v1/forecast" +
            "?latitude=52.52" +
            "&longitude=13.41" +
            "&hourly=temperature_2m";

        var response = await _httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);

        return response.IsSuccessStatusCode
            ? Result.Ok()
            : Result.Fail($"Health check failed with status code: {response.StatusCode}");
    }

    public async Task<Result<GetDailyForecastResponse>> GetDailyForecastAsync(
        double latitude,
        double longitude,
        int forecastDays,
        CancellationToken cancellationToken)
    {
        var url = "v1/forecast" +
            $"?latitude={latitude}" +
            $"&longitude={longitude}" +
            "&daily=temperature_2m_max" +
            $"&forecast_days={forecastDays}" +
            "&timezone=auto";

        var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content
                .ReadFromJsonAsync<GetDailyForecastResponse>(cancellationToken)
                .ConfigureAwait(false);

            if (content is null)
            {
                return new Error("Could not parse the response body.")
                    .WithMetadata("StatusCode", response.StatusCode)
                    .WithMetadata("ResponseContent", await response.Content.ReadAsStringAsync(cancellationToken));
            }

            return Result.Ok(content);
        }

        return new Error("Could not retrieve the daily forecast.")
            .WithMetadata("StatusCode", response.StatusCode)
            .WithMetadata("ResponseContent", await response.Content.ReadAsStringAsync(cancellationToken));
    }
}
