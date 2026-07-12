namespace WebAPI.Features.WeatherForecast.V1.GetWeatherForecast;

internal static class GetWeatherForecastHandler
{
    /// <summary>
    /// Handles the logic for the endpoint that gets the weather forecast.
    /// </summary>
    /// <returns>The endpoint result.</returns>
    public static IResult Handle()
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

        return Results.Ok(new GetWeatherForecastResponse(forecast) { Forecasts = forecast });
    }
}
