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
        var rawData = await _weatherForecastRepository.GetDailyForecastAsync(
            request.Latitude,
            request.Longitude,
            request.ForecastDays,
            cancellationToken);

        if (rawData.IsFailed)
            return rawData.ToResult();

        var forecasts = new List<Domain.WeatherForecast>();

        for (int i = 0; i < rawData.Value.Daily.Time.Length; i++)
        {
            var date = DateOnly.Parse(rawData.Value.Daily.Time[i]);
            var tempC = rawData.Value.Daily.Temperature_2m_Max[i];

            forecasts.Add(new Domain.WeatherForecast(date, tempC));
        }

        return Result.Ok(new GetWeatherForecastResponse(forecasts));
    }
}
