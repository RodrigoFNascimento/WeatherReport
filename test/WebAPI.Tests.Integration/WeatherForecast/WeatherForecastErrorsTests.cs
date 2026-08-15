using Aspire.Hosting.Testing;
using System.Net;
using WebAPI.Tests.Integration.Fixtures;

namespace WebAPI.Tests.Integration.WeatherForecast;

/// <summary>
/// Integration tests for weather forecast error scenarios.
/// </summary>
[Collection("Weather Forecast Tests")]
public sealed class WeatherForecastErrorsTests : IAsyncLifetime
{
    private OpenMeteoFixture? _openMeteoMock;
    private Aspire.Hosting.DistributedApplication? _app;

    public async ValueTask InitializeAsync()
    {
        _openMeteoMock = new OpenMeteoFixture();
        await _openMeteoMock.InitializeAsync();
        _openMeteoMock.SetupHealthCheckMock();
    }

    public async ValueTask DisposeAsync()
    {
        if (_app != null)
            await _app.DisposeAsync();

        if (_openMeteoMock != null)
            await _openMeteoMock.DisposeAsync();
    }

    [Fact]
    public async Task Get_WhenOpenMeteoReturnsInternalServerError_ReturnsInternalServerError()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        _openMeteoMock!.SetupDailyForecastErrorMock(HttpStatusCode.InternalServerError);

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
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.StartsWith("application/problem+json", response.Content.Headers.ContentType?.ToString());
        }
        finally
        {
            Environment.SetEnvironmentVariable("ExternalServices__OpenMeteo__Url", null);
        }
    }
    
    [Fact]
    public async Task Get_WhenOpenMeteoReturnsGatewayTimeout_ReturnsInternalServerError()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        _openMeteoMock!.SetupDailyForecastErrorMock(HttpStatusCode.GatewayTimeout);

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
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.StartsWith("application/problem+json", response.Content.Headers.ContentType?.ToString());
        }
        finally
        {
            Environment.SetEnvironmentVariable("ExternalServices__OpenMeteo__Url", null);
        }
    }
}
