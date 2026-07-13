using MediatR;

namespace Application.UseCases.WeatherForecast.GetWeatherForecast;

public sealed record GetWeatherForecastRequest : IRequest<GetWeatherForecastResponse>;
