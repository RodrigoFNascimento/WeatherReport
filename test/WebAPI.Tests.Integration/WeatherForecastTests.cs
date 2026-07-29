using Aspire.Hosting.Testing;
using System.Net;

namespace WebAPI.Tests.Integration;

public sealed class WeatherForecastTests
{
    [Fact]
    public async Task Get_WhenCalled_ReturnsSuccess()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.WeatherReport_AppHost>(cancellationToken);
        await using var app = await appHost.BuildAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        var client = app.CreateHttpClient("webapi");

        // Act
        var response = await client.GetAsync("v1/weatherforecast", cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }
}
