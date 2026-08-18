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
        var result = await next(cancellationToken);

        if (result.IsSuccess)
            return result;

        var error = result.Errors[0];

        var message = error.Message;
        Dictionary<string, object?> state = [];
        state["@request"] = request;
        state["@result"] = result;

        if (error.Reasons.FirstOrDefault() is ExceptionalError exceptionalError)
        {
            var ex = exceptionalError.Exception;
            state["error.message"] = ex.Message;
            state["error.kind"] = ex.GetType().Name;
            state["error.stack"] = ex.StackTrace;

            using (_logger.BeginScope(state))
                _logger.LogError(ex, "{ErrorMessage}", message);

            return result;
        }

        using (_logger.BeginScope(state))
            _logger.LogError("{ErrorMessage}", message);

        return result;
    }
}
