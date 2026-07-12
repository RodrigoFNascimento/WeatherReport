namespace WebAPI.Features.WeatherForecast.V1.GetWeatherForecast;

internal sealed class GetWeatherForecastEndpoint : IEndpointDefinition
{
    /// <summary>
    /// Maps the endpoint to get the weather forecast.
    /// </summary>
    /// <param name="app">The instance of <see cref="IEndpointRouteBuilder"/> the endpoint will be mapped to.</param>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        app.MapGet("", GetWeatherForecastHandler.Handle)
            .WithName("GetWeatherForecast")
            .WithSummary("Gets the weather forecast.")
            .WithDescription("Gets the weather forecast.")
            .Produces<GetWeatherForecastResponse>()
            .CacheOutput();
    }
}
