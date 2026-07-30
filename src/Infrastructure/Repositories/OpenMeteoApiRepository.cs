using Application.Repositories.WeatherForecast;
using Domain;
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
        var requestUri = "v1/forecast?" +
            "latitude=52.52" +
            "&longitude=13.41" +
            "&hourly=temperature_2m";

        var response = await _httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);

        return response.IsSuccessStatusCode
            ? Result.Ok()
            : Result.Fail($"Health check failed with status code: {response.StatusCode}");
    }

    public async Task<Result<IReadOnlyCollection<WeatherForecast>>> GetDailyForecastAsync(
        double latitude,
        double longitude,
        int forecastDays,
        CancellationToken cancellationToken)
    {
        var url = $"v1/forecast?" +
            $"latitude={latitude}" +
            $"&longitude={longitude}" +
            $"&daily=temperature_2m_max" +
            $"&forecast_days={forecastDays}" +
            $"&timezone=auto";

        var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            return new Error("Could not retrieve daily forecast from provider.")
                .WithMetadata("response.status_code", response.StatusCode)
                .WithMetadata("response.content", await response.Content.ReadAsStringAsync(cancellationToken));
        }

        var content = await response.Content
            .ReadFromJsonAsync<OpenMeteoResponseDto>(cancellationToken)
            .ConfigureAwait(false);

        if (content?.Daily?.Time is null || content.Daily.Temperature_2m_Max is null)
        {
            return Result.Fail("Received invalid or malformed data from weather provider.");
        }

        // Anti-Corruption Layer: Validate and translate provider data to Domain model
        return MapToDomain(content);
    }

    private static Result<IReadOnlyCollection<WeatherForecast>> MapToDomain(OpenMeteoResponseDto dto)
    {
        var times = dto.Daily.Time;
        var temps = dto.Daily.Temperature_2m_Max;

        if (times.Length != temps.Length)
        {
            return new Error("Mismatched data lengths received from weather provider.")
                .WithMetadata("response.content.daily.time.length", dto.Daily.Time.Length)
                .WithMetadata("response.content.daily.temperature_2m_max.length", dto.Daily.Temperature_2m_Max.Length);
        }

        var forecasts = new List<WeatherForecast>(times.Length);

        for (int i = 0; i < times.Length; i++)
        {
            if (!DateOnly.TryParse(times[i], out var date))
            {
                return Result.Fail($"Invalid date format received: {times[i]}");
            }

            var temperature = Temperature.FromCelsius(temps[i]);
            forecasts.Add(new WeatherForecast(date, temperature));
        }

        return Result.Ok<IReadOnlyCollection<WeatherForecast>>(forecasts);
    }

    private sealed record OpenMeteoResponseDto(DailyDataDto Daily);
    private sealed record DailyDataDto(string[] Time, double[] Temperature_2m_Max);
}