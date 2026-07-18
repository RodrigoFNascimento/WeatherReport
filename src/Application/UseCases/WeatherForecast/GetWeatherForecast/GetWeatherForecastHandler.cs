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

        var forecasts = new List<WeatherForecast>();

        for (int i = 0; i < rawData.Value.Daily.Time.Length; i++)
        {
            var date = DateOnly.Parse(rawData.Value.Daily.Time[i]);
            var tempC = (int)Math.Round(rawData.Value.Daily.Temperature_2m_Max[i]);
            var summary = MapTemperatureToSummary(tempC);

            forecasts.Add(new WeatherForecast(date, tempC, summary));
        }

        return Result.Ok(new GetWeatherForecastResponse(forecasts));
    }

    private static string MapTemperatureToSummary(int tempC) => tempC switch
    {
        < -10 => "Freezing",
        >= -10 and < 0 => "Bracing",
        >= 0 and < 10 => "Chilly",
        >= 10 and < 15 => "Cool",
        >= 15 and < 20 => "Mild",
        >= 20 and < 25 => "Warm",
        >= 25 and < 30 => "Balmy",
        >= 30 and < 35 => "Hot",
        >= 35 and < 40 => "Sweltering",
        _ => "Scorching"
    };
}
