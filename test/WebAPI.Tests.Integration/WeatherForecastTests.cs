using Aspire.Hosting.Testing;
using System.Net;
using WebAPI.Tests.Integration.Fixtures;

namespace WebAPI.Tests.Integration;

public sealed class WeatherForecastTests : IAsyncLifetime
{
    private readonly OpenMeteoFixture _openMeteoMock = new();
    private Aspire.Hosting.DistributedApplication? _app;

    public async ValueTask InitializeAsync()
    {
        await _openMeteoMock.InitializeAsync();
        _openMeteoMock.SetupHealthCheckMock();
        _openMeteoMock.SetupDailyForecastMock(
            latitude: 52.52,
            longitude: 13.41,
            forecastDays: 5,
            dates: ["2024-01-01", "2024-01-02", "2024-01-03", "2024-01-04", "2024-01-05"],
            temperatures: [5.2, 6.1, 4.8, 7.3, 5.9]
        );
    }

    public async ValueTask DisposeAsync()
    {
        if (_app != null)
            await _app.DisposeAsync();

        await _openMeteoMock.DisposeAsync();
    }

    [Fact]
    public async Task Get_WhenCalled_ReturnsSuccess()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        Environment.SetEnvironmentVariable("ExternalServices__OpenMeteo__Url", _openMeteoMock.BaseUrl);

        try
        {
            var appHost = await DistributedApplicationTestingBuilder
                .CreateAsync<Projects.WeatherReport_AppHost>(cancellationToken);

            _app = await appHost.BuildAsync(cancellationToken);
            await _app.StartAsync(cancellationToken);

            var client = _app.CreateHttpClient("webapi");

            // Act
            var response = await client.GetAsync("v1/weatherforecast", cancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
        }
        finally
        {
            Environment.SetEnvironmentVariable("ExternalServices__OpenMeteo__Url", null);
        }
    }
}
