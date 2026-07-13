using System.Net.Mime;

namespace WebAPI.Features.WeatherForecast.V1.GetWeatherForecast;

internal sealed class GetWeatherForecastEndpoint : IEndpointDefinition
{
    /// <summary>
    /// Maps the endpoint to get the weather forecast.
    /// </summary>
    /// <param name="app">The instance of <see cref="IEndpointRouteBuilder"/> the endpoint will be mapped to.</param>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("", GetWeatherForecastHandler.Handle)
            .WithName("GetWeatherForecast")
            .WithSummary("Gets the weather forecast.")
            .WithDescription("Gets the weather forecast.")
            .Produces<GetWeatherForecastResponse>()
            .ProducesProblem(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)
            .CacheOutput();
    }
}
