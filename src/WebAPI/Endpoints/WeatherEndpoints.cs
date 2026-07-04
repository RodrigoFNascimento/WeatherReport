using Asp.Versioning.Builder;

namespace WebAPI.Endpoints;

internal static class WeatherEndpoints
{
    /// <summary>
    /// Adds endpoints to return the weather forecast.
    /// </summary>
    /// <param name="endpoints">The <see cref="IVersionedEndpointRouteBuilder"/> the endpoints will be added to.</param>
    /// <returns>The configured <see cref="IVersionedEndpointRouteBuilder"/> for further customization.</returns>
    public static IVersionedEndpointRouteBuilder MapWeatherEndpoints(this IVersionedEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("v{version:apiVersion}/weatherforecast")
            .HasApiVersion(1);

        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        group.MapGet("", () =>
        {
            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            return forecast;
        })
        .WithName("GetWeatherForecast")
        .WithSummary("Gets the weather forecast.")
        .WithDescription("Gets the weather forecast.")
        .MapToApiVersion(1);

        return endpoints;
    }
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
