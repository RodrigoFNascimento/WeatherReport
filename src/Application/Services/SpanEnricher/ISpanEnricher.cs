namespace Application.Services.SpanEnricher;

/// <summary>
/// Handles telemetry spans to add custom data.
/// </summary>
/// <remarks>
/// Abstracts the integration with observability systems
/// providing a decoupled way to register data.
/// </remarks>
public interface ISpanEnricher
{
    /// <summary>
    /// Enriches the span with an <see cref="Exception"/>.
    /// </summary>
    /// <param name="exception">Exception that should enrich the span.</param>
    void EnrichWithException(Exception exception);
}
