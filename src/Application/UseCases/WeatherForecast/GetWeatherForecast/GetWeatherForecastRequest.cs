using FluentResults;
using MediatR;

namespace Application.UseCases.WeatherForecast.GetWeatherForecast;

/// <summary>
/// Request to retrieve a weather forecast for a given geographic location.
/// </summary>
/// <param name="Latitude">Latitude in decimal degrees. Positive values indicate north of the equator, negative values indicate south.</param>
/// <param name="Longitude">Longitude in decimal degrees. Positive values indicate east of the prime meridian, negative values indicate west.</param>
/// <param name="ForecastDays">Number of days to include in the forecast. Must be a positive integer; the handler may impose additional limits.</param>
/// <remarks>
/// This record implements <see cref="IRequest{TResponse}"/> returning a <see cref="Result{TValue}"/> wrapping <see cref="GetWeatherForecastResponse"/>.
/// Use this request with mediator to invoke the corresponding handler in the application pipeline.
/// </remarks>
public sealed record GetWeatherForecastRequest(
    double Latitude,
    double Longitude,
    int ForecastDays)
    : IRequest<Result<GetWeatherForecastResponse>>;