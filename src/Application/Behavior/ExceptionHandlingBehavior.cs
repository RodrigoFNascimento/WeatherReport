using Application.Services.SpanEnricher;
using FluentResults;
using MediatR;

namespace Application.Behavior;

/// <summary>
/// Maps exceptions to a failed <see cref="Result{TResponse}"/>.
/// </summary>
/// <typeparam name="TRequest">Request type.</typeparam>
/// <typeparam name="TResponse">Response type.</typeparam>
internal sealed class ExceptionHandlingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : ResultBase, new()
{
    private readonly ISpanEnricher _spanEnricher;

    public ExceptionHandlingBehavior(ISpanEnricher spanEnricher) =>
        _spanEnricher = spanEnricher;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next(cancellationToken);
        }
        catch (Exception ex)
        {
            _spanEnricher.EnrichWithException(ex);

            var result = new TResponse();
            result.Reasons.Add(new Error(ex.Message).CausedBy(ex));
            return result;
        }
    }
}
