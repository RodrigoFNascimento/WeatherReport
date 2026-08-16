using Application.Repositories.WeatherForecast;
using Application.UseCases.WeatherForecast.GetWeatherForecast;
using FluentResults;
using NSubstitute;
using Domain;

namespace Application.Tests.Unit.UseCases.WeatherForecast;

public sealed class GetWeatherForecastHandlerTests
{
    private readonly IWeatherForecastRepository _weatherForecastRepository;
    private readonly GetWeatherForecastHandler _sut;

    public GetWeatherForecastHandlerTests()
    {
        _weatherForecastRepository = Substitute.For<IWeatherForecastRepository>();
        _sut = new(_weatherForecastRepository);
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsForecast_ShouldReturnForecast()
    {
        // Arrange
        var request = new GetWeatherForecastRequest(40.7128, -74.0060, 5);
        var expectedForecasts = new List<Domain.WeatherForecast>
        {
            new(DateOnly.FromDateTime(DateTime.Now), Temperature.FromCelsius(20)),
            new(DateOnly.FromDateTime(DateTime.Now.AddDays(1)), Temperature.FromCelsius(22)),
            new(DateOnly.FromDateTime(DateTime.Now.AddDays(2)), Temperature.FromCelsius(18)),
            new(DateOnly.FromDateTime(DateTime.Now.AddDays(3)), Temperature.FromCelsius(25)),
            new(DateOnly.FromDateTime(DateTime.Now.AddDays(4)), Temperature.FromCelsius(15))
        };

        _weatherForecastRepository.GetDailyForecastAsync(
            request.Latitude,
            request.Longitude,
            request.ForecastDays,
            Arg.Any<CancellationToken>())
            .Returns(Result.Ok<IReadOnlyCollection<Domain.WeatherForecast>>(expectedForecasts));

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedForecasts, result.Value.Forecasts);
    }
}
