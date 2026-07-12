using Asp.Versioning;

namespace WebAPI.Features.WeatherForecast;

internal static class WeatherForecastGroup
{
    /// <summary>
    /// Maps the weather forecast endpoints.
    /// </summary>
    /// <param name="app">The instance of <see cref="IEndpointRouteBuilder"/> the endpoints will be mapped to.</param>
    /// <returns>The instance of <see cref="IEndpointRouteBuilder"/> for further configuration.</returns>
    public static IEndpointRouteBuilder MapWeatherForecastEndpoints(this IEndpointRouteBuilder app)
    {
        var apiVersionSet = app.NewApiVersionSet("WeatherForecast")
            .HasApiVersion(new ApiVersion(1, 0))
            .Build();

        var group = app.MapGroup("v{version:apiVersion}/weatherforecast")
            .WithApiVersionSet(apiVersionSet)
            .WithTags("WeatherForecast");

        var v1Group = group.MapToApiVersion(1, 0);
        new V1.GetWeatherForecast.GetWeatherForecastEndpoint().MapEndpoint(v1Group);

        return app;
    }
}
