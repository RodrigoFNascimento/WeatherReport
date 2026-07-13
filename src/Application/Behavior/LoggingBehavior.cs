using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Behavior;

/// <summary>
/// Logs <typeparamref name="TResponse"/> if failed.
/// </summary>
/// <typeparam name="TRequest">Request type.</typeparam>
/// <typeparam name="TResponse">Response type.</typeparam>
internal sealed class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : ResultBase
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next(cancellationToken);

        if (response.IsSuccess)
            return response;

        var error = response.Errors[0];

        var message = error.Reasons.FirstOrDefault()?.Message ?? error.Message;

        if (error.Reasons.FirstOrDefault() is ExceptionalError exceptionalError)
        {
            Dictionary<string, object?> state = [];
            var ex = exceptionalError.Exception;
            state["error.message"] = ex.Message;
            state["error.kind"] = ex.GetType().Name;
            state["error.stack"] = ex.StackTrace;

            using (_logger.BeginScope(state))
                _logger.LogError(ex, "{ErrorMessage}", message);

            return response;
        }

        _logger.LogError("{ErrorMessage}", message);

        return response;
    }
}
