using Application.Repositories.WeatherForecast;
using FluentResults;
using MediatR;

namespace Application.UseCases.WeatherForecast.GetWeatherForecast;

internal sealed class GetWeatherForecastHandler
    : IRequestHandler<GetWeatherForecastRequest, Result<GetWeatherForecastResponse>>
{
    private readonly IWeatherForecastRepository _weatherForecastRepository;

    public GetWeatherForecastHandler(IWeatherForecastRepository weatherForecastRepository) =>
        _weatherForecastRepository = weatherForecastRepository;

    public async Task<Result<GetWeatherForecastResponse>> Handle(
        GetWeatherForecastRequest request,
        CancellationToken cancellationToken)
    {
        var forecastResult = await _weatherForecastRepository.GetDailyForecastAsync(
            request.Latitude,
            request.Longitude,
            request.ForecastDays,
            cancellationToken);

        if (forecastResult.IsFailed)
            return forecastResult.ToResult();

        return Result.Ok(new GetWeatherForecastResponse(forecastResult.Value));
    }
}