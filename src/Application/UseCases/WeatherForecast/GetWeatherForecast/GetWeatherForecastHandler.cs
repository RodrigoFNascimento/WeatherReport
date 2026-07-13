using MediatR;

namespace Application.UseCases.WeatherForecast.GetWeatherForecast;

internal sealed class GetWeatherForecastHandler
    : IRequestHandler<GetWeatherForecastRequest, GetWeatherForecastResponse>
{
    public Task<GetWeatherForecastResponse> Handle(
        GetWeatherForecastRequest request,
        CancellationToken cancellationToken)
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();

        return Task.FromResult(new GetWeatherForecastResponse(forecast));
    }
}
