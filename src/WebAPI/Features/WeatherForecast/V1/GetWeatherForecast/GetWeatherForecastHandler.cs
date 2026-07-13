using Application.UseCases.WeatherForecast.GetWeatherForecast;
using FluentResults.HttpMapping.Execution;
using MediatR;

namespace WebAPI.Features.WeatherForecast.V1.GetWeatherForecast;

internal static class GetWeatherForecastHandler
{
    /// <summary>
    /// Handles the logic for the endpoint that gets the weather forecast.
    /// </summary>
    /// <param name="sender">Sends a mediator request.</param>
    /// <param name="httpResultMapper">Maps a result to an HTTP response.</param>
    /// <returns>The endpoint result.</returns>
    public static async Task<IResult> Handle(ISender sender, IHttpResultMapper httpResultMapper)
    {
        var result = await sender.Send(new GetWeatherForecastRequest());

        var presentationResponse = result.Map(
            x => new GetWeatherForecastResponse(
                x.Forecasts.Select(
                    f => new WeatherForecast(f.Date, f.TemperatureC, f.TemperatureF, f.Summary))));

        return httpResultMapper.Map(presentationResponse);
    }
}
