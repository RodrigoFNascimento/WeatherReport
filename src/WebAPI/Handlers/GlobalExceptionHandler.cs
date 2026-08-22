using Application.Services.SpanEnricher;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Handlers;

internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly ISpanEnricher _spanEnricher;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IProblemDetailsService problemDetailsService,
        ISpanEnricher spanEnricher)
    {
        _logger = logger;
        _problemDetailsService = problemDetailsService;
        _spanEnricher = spanEnricher;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        const int ResponseStatusCode = StatusCodes.Status500InternalServerError;

        _logger.LogError(exception, "{Message}", exception.Message);
        _spanEnricher.EnrichWithException(exception);

        httpContext.Response.StatusCode = ResponseStatusCode;

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = ResponseStatusCode,
                Title = "Unexpected internal error.",
                Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                Detail = "An unexpected internal error occurred. Try again later."
            }
        });
    }
}
