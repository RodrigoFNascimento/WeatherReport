using System.Net;
using System.Text.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace WebAPI.Tests.Integration.Fixtures;

/// <summary>
/// Fixture for managing the server that stubs the Open Meteo API.
/// </summary>
public sealed class OpenMeteoFixture : IAsyncLifetime
{
    private WireMockServer? _server;

    /// <summary>
    /// Gets the base URL of the server.
    /// </summary>
    public string BaseUrl => _server?.Urls.FirstOrDefault()
        ?? throw new InvalidOperationException("WireMock server is not initialized");

    /// <summary>
    /// Initializes the server asynchronously.
    /// </summary>
    public ValueTask InitializeAsync()
    {
        _server = WireMockServer.Start()
            ?? throw new InvalidOperationException("Failed to start WireMock server");

        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Disposes the server asynchronously.
    /// </summary>
    public ValueTask DisposeAsync()
    {
        if (_server != null)
        {
            _server.Stop();
            _server.Dispose();
        }

        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Sets up a mock response for the daily weather forecast endpoint.
    /// </summary>
    /// <param name="latitude">The latitude parameter from the request.</param>
    /// <param name="longitude">The longitude parameter from the request.</param>
    /// <param name="forecastDays">The forecast days parameter from the request.</param>
    /// <param name="dates">Array of date strings (YYYY-MM-DD format).</param>
    /// <param name="temperatures">Array of temperature values in Celsius.</param>
    public void SetupDailyForecastMock(
        double latitude,
        double longitude,
        int forecastDays,
        string[] dates,
        double[] temperatures)
    {
        if (dates.Length != temperatures.Length)
            throw new ArgumentException("Dates and temperatures arrays must have the same length");

        var responseBody = new
        {
            daily = new
            {
                time = dates,
                temperature_2m_max = temperatures
            }
        };

        var request = Request
            .Create()
            .WithPath("/v1/forecast")
            .WithParam("latitude", latitude.ToString())
            .WithParam("longitude", longitude.ToString())
            .WithParam("daily", "temperature_2m_max")
            .WithParam("forecast_days", forecastDays.ToString())
            .WithParam("timezone", "auto");

        var response = Response
            .Create()
            .WithStatusCode(HttpStatusCode.OK)
            .WithHeader("Content-Type", "application/json")
            .WithBody(JsonSerializer.Serialize(responseBody));

        _server?.Given(request)
            .RespondWith(response);
    }

    /// <summary>
    /// Sets up a mock response for the health check endpoint.
    /// </summary>
    public void SetupHealthCheckMock()
    {
        var responseBody = new
        {
            hourly = new
            {
                temperature_2m = Array.Empty<double>()
            }
        };

        var request = Request
            .Create()
            .WithPath("/v1/forecast")
            .WithParam("latitude", "52.52")
            .WithParam("longitude", "13.41")
            .WithParam("hourly", "temperature_2m");

        var response = Response
            .Create()
            .WithStatusCode(HttpStatusCode.OK)
            .WithHeader("Content-Type", "application/json")
            .WithBody(JsonSerializer.Serialize(responseBody));

        _server?.Given(request)
            .RespondWith(response);
    }
}
