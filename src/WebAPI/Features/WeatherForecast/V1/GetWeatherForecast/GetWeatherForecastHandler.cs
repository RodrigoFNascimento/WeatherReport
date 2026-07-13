using Application.UseCases.WeatherForecast.GetWeatherForecast;
using MediatR;

namespace WebAPI.Features.WeatherForecast.V1.GetWeatherForecast;

internal static class GetWeatherForecastHandler
{
    /// <summary>
    /// Handles the logic for the endpoint that gets the weather forecast.
    /// </summary>
    /// <returns>The endpoint result.</returns>
    public static async Task<IResult> Handle(ISender sender)
    {
        var result = await sender.Send(new GetWeatherForecastRequest());

        var forecasts = result.Forecasts
            .Select(f => new WeatherForecast(f.Date, f.TemperatureC, f.TemperatureF, f.Summary));

        return Results.Ok(new GetWeatherForecastResponse(forecasts));
    }
}
