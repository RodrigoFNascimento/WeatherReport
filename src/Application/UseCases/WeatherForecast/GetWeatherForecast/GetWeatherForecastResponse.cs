namespace Application.UseCases.WeatherForecast.GetWeatherForecast;

/// <summary>
/// Response returned by the <c>GetWeatherForecast</c> use case.
/// </summary>
/// <param name="Forecasts">An enumerable of <see cref="Domain.WeatherForecast"/> items representing the requested forecasts.</param>
public sealed record GetWeatherForecastResponse(IEnumerable<Domain.WeatherForecast> Forecasts);
